using DaticianProj.Core.Application.Interfaces.Repositories;
using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Core.Domain.Entities;
using DaticianProj.Models;
using KonsumeTestRun.Core.Application.Interfaces.Repositories;
using System.Security.Claims;
using DaticianProj.Models.ProfileModel;
using DaticianProj.Core.Domain.Enum;
using MailKit;
using Microsoft.AspNetCore.Http.HttpResults;
using DaticianProj.Models.UserModel;

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
                    Age = DateTime.UtcNow.Year - profile.DateOfBirth.Year,
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
                    Age = DateTime.UtcNow.Year - profile.DateOfBirth.Year,
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

        public async Task<BaseResponse<ProfileResponse>> CreateProfile(int Userid, ProfileRequest request)
        {
            var user = await _userRepository.GetAsync(Userid);
            if (user == null)
            {
                return new BaseResponse<ProfileResponse>
                {
                    Message = "User does not exist",
                    IsSuccessful = false
                };
            }

            var role = await _roleRepository.GetAsync(r => r.Name.ToLower() == "patient");
            if (role == null)
            {
                return new BaseResponse<ProfileResponse>
                {
                    Message = $"Role with id '{role.Id}' does not exists",
                    IsSuccessful = false
                };
            }
            var profile = user.Profile ?? new Profile();
            profile.DateOfBirth = DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc);
            profile.Gender = (Domain.Enum.Gender)(int)request.Gender;
            profile.Height = request.Height;
            profile.Weight = request.Weight;
            profile.UserGoals = request.UserGoals;
            profile.Allergies = request.Allergies;
            profile.BodyFat = request.BodyFat;
            profile.Nationality = request.Nationality;
            profile.DietType = request.DietType;
            profile.NoOfMealPerDay = request.NoOfMealPerDay;
            profile.SnackPreference = request.SnackPreference;
            profile.UserId = Userid;
            profile.User = user;
            profile.DateCreated = DateTime.UtcNow;
            profile.IsDeleted = false;
            profile.CreatedBy = "1";

            
            try
            {
                if (user.Profile == null)
                    await _profileRepository.AddAsync(profile);
                _roleRepository.Update(role);
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
                    Age = DateTime.UtcNow.Year - profile.DateOfBirth.Year,
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


        public async Task<BaseResponse> UpdateProfile(int id, ProfileRequest request)
        {
            var profile = await _profileRepository.GetAsync(id);
            var user = await _userRepository.GetAsync(profile.User.Email);
            if (profile == null)
            {
                return new BaseResponse
                {
                    Message = "profile does not exist",
                    IsSuccessful = false
                };
            }

            var formerRole = await _roleRepository.GetAsync(user.RoleId);
            formerRole.Users.Remove(user);
            _roleRepository.Update(formerRole);

            var exists = await _profileRepository.ExistsAsync(profile.User.Email, id);
            if (exists)
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

            var loginUserId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            profile.DateOfBirth = DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc);
            profile.Gender = (Domain.Enum.Gender)(int)request.Gender;
            profile.Height = request.Height;
            profile.Weight = request.Weight;
            profile.UserGoals = request.UserGoals;
            profile.Allergies = request.Allergies;
            profile.BodyFat = request.BodyFat;
            profile.Nationality = request.Nationality;
            profile.DietType = request.DietType;
            profile.NoOfMealPerDay = request.NoOfMealPerDay;
            profile.SnackPreference = request.SnackPreference;
            profile.UserId = user.Id;
            profile.User = user;
            profile.ModifiedBy = loginUserId;
            profile.DateModified = DateTime.UtcNow;
            profile.IsDeleted = false;
            profile.CreatedBy = "1";

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
