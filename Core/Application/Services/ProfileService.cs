using DaticianProj.Core.Application.Interfaces.Repositories;
using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Core.Domain.Entities;
using DaticianProj.Models.UserModel;
using DaticianProj.Models;
using Google.Apis.Auth;
using KonsumeTestRun.Core.Application.Interfaces.Repositories;
using Project.Models.Entities;
using System.Security.Claims;
using DaticianProj.Models.ProfileModel;

namespace DaticianProj.Core.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly IEmailService _emailService;
        public ProfileService(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContext, IVerificationCodeRepository verificationCodeRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
            _verificationCodeRepository = verificationCodeRepository;
            _emailService = emailService;
        }

        public async Task<BaseResponse<ProfileResponse>> CreateProfile(UserRequest request)
        {
            var exists = await _userRepository.ExistsAsync(request.Email);
            if (exists)
            {
                return new BaseResponse<ProfileResponse>
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new BaseResponse<ProfileResponse>
                {
                    Message = "Password does not match",
                    IsSuccessful = false
                };
            }

            var role = await _roleRepository.GetAsync(r => r.Name.ToLower() == "patient");
            if (role == null)
            {
                return new BaseResponse<ProfileResponse>
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
                return new BaseResponse<ProfileResponse>
                {
                    Message = $"An error occurred while sending email{ex.Message}",
                    IsSuccessful = false
                };
            }
            await _unitOfWork.SaveAsync();
            return new BaseResponse<ProfileResponse>
            {
                Message = "Check Your Mail And Complete Your Registration",
                IsSuccessful = true,
                Value = new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Age = DateTime.Now.Year - user.DateOfBirth.Year,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    RoleName = user.Role.Name,
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

        public async Task<BaseResponse<ICollection<ProfileResponse>>> GetAllProfiles()
        {
            var users = await _userRepository.GetAllAsync();

            return new BaseResponse<ICollection<ProfileResponse>>
            {
                Message = "List of users",
                IsSuccessful = true,
                Value = users.Select(user => new ProfileResponse
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Age = DateTime.Now.Year - user.DateOfBirth.Year,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    RoleName = user.Role.Name,
                    DateOfBirth = user.DateOfBirth,
                    Gender = (Domain.Enum.Gender)(int)user.Gender,
                    Height = user.Height,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    Weight = user.Weight,
                    UserGoal = user.UserGoal,
                }).ToList(),
            };
        }

        public async Task<BaseResponse<ProfileResponse>> GetProfile(int id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return new BaseResponse<ProfileResponse>
                {
                    Message = "User not found",
                    IsSuccessful = false
                };
            }
            var role = await _roleRepository.GetAsync(r => r.Name.ToLower() == "patient");
            return new BaseResponse<ProfileResponse>
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

        public async Task<BaseResponse> RemoveProfile(int id)
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

        public async Task<BaseResponse> UpdateProfile(int id, UserRequest request)
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

        
    }
}
