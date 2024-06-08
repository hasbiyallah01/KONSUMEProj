using DaticianProj.Core.Application.Interfaces.Repositories;
using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Core.Domain.Entities;
using DaticianProj.Core.Domain.Enum;
using DaticianProj.Models;
using DaticianProj.Models.UserModel;
using Google.Apis.Auth;
using KonsumeTestRun.Core.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Project.Models.Entities;
using sib_api_v3_sdk.Model;
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
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly IEmailService _emailService;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContext, IVerificationCodeRepository verificationCodeRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
            _verificationCodeRepository = verificationCodeRepository;
            _emailService = emailService;
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
        private static readonly Random random = new Random();

        public async Task<BaseResponse<UserResponse>> CreateUser(UserRequest request)
        {
            int randomCode = random.Next(10000, 99999);
            var exists = await _userRepository.ExistsAsync(request.Email);
            if (exists)
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "Password does not match",
                    IsSuccessful = false
                };
            }

            var role = await _roleRepository.GetAsync(r => r.Name.ToLower() == "patient");
            if (role == null)
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "Role does not exist",
                    IsSuccessful = false
                };
            }

            var user = new User
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                RoleId = role.Id,
                Role = role,
                CreatedBy = "1"
            };

            role.Users.Add(user);
            _roleRepository.Update(role);
            var newUser = await _userRepository.AddAsync(user);
            var code = new VerificationCode
            {
                Code = randomCode,
                UserId = newUser.Id
            };
            await _verificationCodeRepository.Create(code);

            try
            {
                var mailRequest = new MailRequests
                {
                    Subject = "Confirmation Code",
                    ToEmail = user.Email,
                    Title = "Your Confirmation Code",
                    HtmlContent = $"<html><body><h1>Hello {user.FirstName}, Welcome KONSUME.</h1><h4>Your confirmation code is {code.Code} to continue with the registration</h4></body></html>",
                };

                await _emailService.SendEmailAsync(new MailRecieverDto { Name = user.FirstName, Email = user.Email }, mailRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while sending email: {ex.Message}");
                return new BaseResponse<UserResponse>
                {
                    Message = $"An error occurred while sending email{ex.Message}",
                    IsSuccessful = false
                };
            }
            await _unitOfWork.SaveAsync();
            return new BaseResponse<UserResponse>
            {
                Message = "Check Your Mail And Complete Your Registration",
                IsSuccessful = true,
                Value = new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    RoleName = user.Role.Name,
                    Role = user.Role,
                }
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
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
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
                    Email = user.Email,
                    RoleId = user.RoleId,
                    RoleName = user.Role.Name,
                    Role = user.Role,
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
                    Email = user.Email,
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Role = user.Role,
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
                    Email = user.Email,
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Role = user.Role,
                }
            };
        }
    }
}
