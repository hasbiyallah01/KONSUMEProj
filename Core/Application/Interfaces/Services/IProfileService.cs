using DaticianProj.Models.UserModel;
using DaticianProj.Models;
using DaticianProj.Models.ProfileModel;

namespace DaticianProj.Core.Application.Interfaces.Services
{
    public interface IProfileService
    {
        Task<BaseResponse<ProfileResponse>> GetProfile(int id);
        Task<BaseResponse<ICollection<ProfileResponse>>> GetAllProfiles();
        Task<BaseResponse> RemoveProfile(int id);
        Task<BaseResponse> UpdateProfile(int id, ProfileRequest request);
        Task<BaseResponse<ProfileResponse>> CreateProfile(ProfileRequest request);
    }
}
