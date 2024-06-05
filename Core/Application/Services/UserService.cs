using DaticianProj.Core.Application.Interfaces.Repositories;
using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Core.Domain.Entities;
using DaticianProj.Infrastructure.Repositories;
using DaticianProj.Models;
using DaticianProj.Models.UserModel;
using Google.Apis.Auth;
using KonsumeTestRun.Core.Application.Interfaces.Repositories;
using Org.BouncyCastle.Asn1.Ocsp;
using Project.Models.Entities;
using System.Security.Claims;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace DaticianProj.Core.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContext,IMailService mailService, IVerificationCodeRepository verificationCodeRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
            _mailService = mailService;
            _verificationCodeRepository = verificationCodeRepository;
        }

        public async Task<GoogleUser> ValidateToken(string token)
        {
            GoogleJsonWebSignature.Payload payload;

            payload = await ValidateAsync(token);
            return new GoogleUser
            {
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
            };
        }

        

        public async Task<BaseResponse> CreateUser(UserRequest request)
        {
            int random = new Random().Next(10000, 99999);
            var exists = await _userRepository.ExistsAsync(request.Email);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new BaseResponse
                {
                    Message = "Password does not match",
                    IsSuccessful = false
                };
            }

            var role = await _roleRepository.GetAsync(r => r.Name.ToLower() == "patient");
            if (role == null)
            {
                return new BaseResponse
                {
                    Message = "Role does not exists",
                    IsSuccessful = false
                };
            }

            var user = new User
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                DateCreated = DateTime.UtcNow,
                Gender = (Domain.Enum.Gender)request.Gender,
                Height = request.Height,
                IsDeleted = false,
                PhoneNumber = request.PhoneNumber,
                UserGoal = request.UserGoal,
                Weight = request.Weight,
                RoleId = role.Id,
                Role = role,
                CreatedBy = "1"
            };

            role.Users.Add(user);
            _roleRepository.Update(role);
            var newUser = await _userRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();
            var code = new VerificationCode
            {
                Code = random,
                UserId = user.Id
            };
            await _verificationCodeRepository.Create(code);
            await _verificationCodeRepository.Save();


            var mailRequest = new MailRequest
            {
                Subject = "Confirmation Code",
                ToEmail = user.Email,
                ToName = user.FirstName,
                HtmlContent = $"<html><body><h1>Hello {user.FirstName}, Welcome KONSUME.</h1><h4>Your confirmation code is {code.Code} to continue with the registration</h4></body></html>",
            };
            _mailService.SendEmail(mailRequest);
            return new BaseResponse
            {
                Message = "User created successfully",
                IsSuccessful = true
            };
        }



        public async Task<BaseResponse> CreateUserUsingAuthAsync(string token, UserRequest request)
        {
            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await ValidateAsync(token);
            }
            catch (InvalidJwtException)
            {
                return null;
            }

            var googleUser = new UserResponse
            {
                Email = payload.Email,
                FullName = payload.GivenName + " " + payload.FamilyName,
            };

            var exists = await _userRepository.ExistsAsync(request.Email);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new BaseResponse
                {
                    Message = "Password does not match",
                    IsSuccessful = false
                };
            }

            var role = await _roleRepository.GetAsync(r => r.Name.ToLower() == "patient");
            if (role == null)
            {
                return new BaseResponse
                {
                    Message = "Role does not exists",
                    IsSuccessful = false
                };
            }
            var user = new User
            {
                Email = payload.Email,
                Password = request.Password,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                DateOfBirth = request.DateOfBirth,
                DateCreated = DateTime.UtcNow,
                Gender = (Domain.Enum.Gender)(int)request.Gender,
                Height = request.Height,
                IsDeleted = false,
                PhoneNumber = request.PhoneNumber,
                UserGoal = request.UserGoal,
                Weight = request.Weight,
                RoleId = role.Id,
                Role = role,
                CreatedBy = "1"
            };

            role.Users.Add(user);
            _roleRepository.Update(role);
            var newUser = await _userRepository.AddAsync(user);

           
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "User created successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse<ICollection<UserResponse>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();

            return new BaseResponse<ICollection<UserResponse>>
            {
                Message = "List of users",
                IsSuccessful = true,
                Value = users.Select(user => new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Age = DateTime.Now.Year - user.DateOfBirth.Year,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    RoleName = user.Role.Name,
                    DateOfBirth = user.DateOfBirth,
                    Gender =(Domain.Enum.Gender)(int) user.Gender,
                    Height = user.Height,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    Weight = user.Weight,
                    UserGoal = user.UserGoal,
                }).ToList(),
            };
        }

        public async Task<BaseResponse<UserResponse>> GetUser(int id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "User not found",
                    IsSuccessful = false
                };
            }
            var role = await _roleRepository.GetAsync(r => r.Name.ToLower() == "patient");
            return new BaseResponse<UserResponse>
            {
                Message = "User successfully found",
                IsSuccessful = true,
                Value = new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Age = DateTime.Now.Year - user.DateOfBirth.Year,
                    Email = user.Email,
                    RoleId = role.Id,
                    RoleName = role.Name,
                    DateOfBirth = user.DateOfBirth,
                    Gender = (Domain.Enum.Gender)(int)user.Gender,
                    Height = user.Height,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    Weight = user.Weight,
                    UserGoal = user.UserGoal,
                }
            };
        }

        public async Task<BaseResponse> RemoveUser(int id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return new BaseResponse
                {
                    Message = "User does not exist",
                    IsSuccessful = false
                };
            }

            _userRepository.Remove(user);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "User deleted successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> UpdateUser(int id, UserRequest request)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return new BaseResponse
                {
                    Message = "User does not exist",
                    IsSuccessful = false
                };
            }

            var formerRole = await _roleRepository.GetAsync(user.RoleId);
            formerRole.Users.Remove(user);
            _roleRepository.Update(formerRole);

            var exists = await _userRepository.ExistsAsync(request.Email, id);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new BaseResponse
                {
                    Message = "Password does not match",
                    IsSuccessful = false
                };
            }

            var role = await _roleRepository.GetAsync(r => r.Name.ToLower() == "patient");
            if (role == null)
            {
                return new BaseResponse
                {
                    Message = $"Role with id '{role.Id}' does not exists",
                    IsSuccessful = false
                };
            }

            var loginUserId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Password = request.Password;
            user.DateOfBirth = request.DateOfBirth;
            user.PhoneNumber = request.PhoneNumber;
            user.Gender = (Domain.Enum.Gender)(int)request.Gender;
            user.IsDeleted = false;
            user.RoleId = role.Id;
            user.Role = role;
            user.DateModified = DateTime.Now;
            user.ModifiedBy = loginUserId;

            role.Users.Add(user);

            _roleRepository.Update(role);
            _userRepository.Update(user);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "User updated successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse<UserResponse>> Login(LoginRequestModel model)
        {
            var user = await _userRepository.GetAsync(user => user.Email == model.Email && user.Password == model.Password);
            if (user == null)
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "Invalid Credentials",
                    IsSuccessful = false
                };
            }
            var role = await _roleRepository.GetAsync(r => r.Name.ToLower() == "patient");
            return new BaseResponse<UserResponse>
            {
                Message = "Login Successfull",
                IsSuccessful = true,
                Value = new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Age = DateTime.Now.Year - user.DateOfBirth.Year,
                    Email = user.Email,
                    RoleId = role.Id,
                    RoleName = role.Name,
                    DateOfBirth = user.DateOfBirth,
                    Gender = (Domain.Enum.Gender)(int)user.Gender,
                    Height = user.Height,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    Weight = user.Weight,
                    UserGoal = user.UserGoal,
                }
            };
        }
    }
}
