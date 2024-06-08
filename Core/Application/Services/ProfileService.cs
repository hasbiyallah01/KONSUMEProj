using DaticianProj.Core.Application.Interfaces.Repositories;
using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Core.Domain.Entities;
using DaticianProj.Models;
using KonsumeTestRun.Core.Application.Interfaces.Repositories;
using System.Security.Claims;
using DaticianProj.Models.ProfileModel;

namespace DaticianProj.Core.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly IEmailService _emailService;
        public ProfileService(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContext, IVerificationCodeRepository verificationCodeRepository, IEmailService emailService, 
            IProfileRepository profileRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
            _verificationCodeRepository = verificationCodeRepository;
            _emailService = emailService;
            _profileRepository = profileRepository;
        }

        public async Task<BaseResponse<ProfileResponse>> CreateProfile(ProfileRequest request)
        {
            var loginUserId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            var user = await _userRepository.GetAsync(loginUserId);
            if (user != null)
            {
                return new BaseResponse<ProfileResponse>
                {
                    Message = "Email already exists!!!",
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

            var profile = new Profile
            {
                DateCreated = DateTime.UtcNow,
                Gender = (Domain.Enum.Gender)(int)request.Gender,
                Height = request.Height,
                IsDeleted = false,
                UserGoals = request.UserGoals,
                Weight = request.Weight,
                Allergies = request.Allergies,
                BodyFat = request.BodyFat,
                DateOfBirth = DateTime.UtcNow,
                DietType = request.DietType,
                Nationality = request.Nationality,
                NoOfMealPerDay = request.NoOfMealPerDay,
                SnackPreference = request.SnackPreference,
                UserId = user.Id,
                User = user,
                CreatedBy = "1"
            };

            role.Users.Add(user);
            _roleRepository.Update(role);
            var newUser = await _profileRepository.AddAsync(profile);
            

            try
            {
                await _emailService.SendNotificationToUserAsync(profile);
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
                Value = new ProfileResponse
                {
                    Id = profile.Id,
                    Age = DateTime.Now.Year - profile.DateOfBirth.Year,
                    Email = user.Email,
                    DateOfBirth = profile.DateOfBirth,
                    Gender = (Domain.Enum.Gender)(int)profile.Gender,
                    Height = profile.Height,
                    Weight = profile.Weight,
                    UserGoals = profile.UserGoals,
                    Allergies = profile.Allergies,
                    BodyFat = profile.BodyFat,
                    Nationality = profile.Nationality,
                    DietType = profile.DietType,
                    NoOfMealPerDay = profile.NoOfMealPerDay,
                    SnackPreference = profile.SnackPreference,
                }
            };
        }

        public async Task<BaseResponse<ICollection<ProfileResponse>>> GetAllProfiles()
        {
            var profile = await _profileRepository.GetAllAsync();

            return new BaseResponse<ICollection<ProfileResponse>>
            {
                Message = "List of users",
                IsSuccessful = true,
                Value = profile.Select(profile => new ProfileResponse
                {
                    Id = profile.Id,
                    Age = DateTime.Now.Year - profile.DateOfBirth.Year,
                    Email = profile.User.Email,
                    DateOfBirth = profile.DateOfBirth,
                    Gender = (Domain.Enum.Gender)(int)profile.Gender,
                    Height = profile.Height,
                    Weight = profile.Weight,
                    UserGoals = profile.UserGoals,
                    Allergies = profile.Allergies,
                    BodyFat = profile.BodyFat,
                    Nationality = profile.Nationality,
                    DietType = profile.DietType,
                    NoOfMealPerDay = profile.NoOfMealPerDay,
                    SnackPreference = profile.SnackPreference,
                }).ToList(),
            };
        }

        public async Task<BaseResponse<ProfileResponse>> GetProfile(int id)
        {
            var profile = await _profileRepository.GetAsync(id);
            if (profile == null)
            {
                return new BaseResponse<ProfileResponse>
                {
                    Message = "User not found",
                    IsSuccessful = false
                };
            }
            return new BaseResponse<ProfileResponse>
            {
                Message = "User successfully found",
                IsSuccessful = true,
                Value = new ProfileResponse
                {
                    Id = profile.Id,
                    Age = DateTime.Now.Year - profile.DateOfBirth.Year,
                    Email = profile.User.Email,
                    DateOfBirth = profile.DateOfBirth,
                    Gender = (Domain.Enum.Gender)(int)profile.Gender,
                    Height = profile.Height,
                    Weight = profile.Weight,
                    UserGoals = profile.UserGoals,
                    Allergies = profile.Allergies,
                    BodyFat = profile.BodyFat,
                    Nationality = profile.Nationality,
                    DietType = profile.DietType,
                    NoOfMealPerDay = profile.NoOfMealPerDay,
                    SnackPreference = profile.SnackPreference,
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

        public async Task<BaseResponse> UpdateProfile(int id, ProfileRequest request)
        {
            var profile = await _profileRepository.GetAsync(id);
            if (profile == null)
            {
                return new BaseResponse
                {
                    Message = "User does not exist",
                    IsSuccessful = false
                };
            }

            var formerRole = await _roleRepository.GetAsync(profile.User.RoleId);
            formerRole.Users.Remove(profile.User);
            _roleRepository.Update(formerRole);
            if (profile != null)
            {
                return new BaseResponse
                {
                    Message = "Email already exists!!!",
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
            profile.DateOfBirth = profile.DateOfBirth;
            profile.Gender = (Domain.Enum.Gender)(int)profile.Gender;
            profile.Height = profile.Height;
            profile.Weight = profile.Weight;
            profile.UserGoals = profile.UserGoals;
            profile.Allergies = profile.Allergies;
            profile.BodyFat = profile.BodyFat;
            profile.Nationality = profile.Nationality;
            profile.DietType = profile.DietType;
            profile.NoOfMealPerDay = profile.NoOfMealPerDay;
            profile.SnackPreference = profile.SnackPreference;
            _roleRepository.Update(role);
            _profileRepository.Update(profile);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "User updated successfully",
                IsSuccessful = true
            };
        }
    }
}
