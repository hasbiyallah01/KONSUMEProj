using DaticianProj.Models.UserModel;
using DaticianProj.Models;
using DaticianProj.Models.ProfileModel;

namespace DaticianProj.Core.Application.Interfaces.Services
{
    public interface IProfileService
    {
        Task<BaseResponse<ProfileResponse>> GetProfile(int id);
        Task<BaseResponse<ICollection<ProfileResponse>>> GetAllProfiles();
        Task<BaseResponse> RemoveUser(int id);
        Task<BaseResponse> UpdateProfile(int id, UserRequest request);
        Task<BaseResponse<ProfileResponse>> CreateProfile(ProfileRequest request);
    }
}
