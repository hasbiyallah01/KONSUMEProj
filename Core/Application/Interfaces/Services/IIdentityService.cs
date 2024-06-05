using DaticianProj.Models.UserModel;

namespace DaticianProj.Core.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        string GenerateToken(string key, string issuer, UserResponse user);
        bool IsTokenValid(string key, string issuer, string token);
    }
}
