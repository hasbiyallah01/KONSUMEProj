

using DaticianProj.Models;
using DaticianProj.Models.UserModel;

namespace DaticianProj.Core.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<BaseResponse<UserResponse>> GetUser(int id);
        Task<BaseResponse<ICollection<UserResponse>>> GetAllUsers();
        Task<BaseResponse> RemoveUser(int id);
        Task<BaseResponse> UpdateUser(int id, UserRequest request);
        Task<BaseResponse<UserResponse>> Login(LoginRequestModel model);
        Task<BaseResponse> CreateUserUsingAuthAsync(string token, UserRequest request);
        Task<GoogleUser > ValidateToken(string token);
        Task<BaseResponse<UserResponse>> CreateUser(UserRequest request);
    }
}
