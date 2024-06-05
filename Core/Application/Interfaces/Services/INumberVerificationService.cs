using DaticianProj.Models;

namespace DaticianProj.Core.Application.Interfaces.Services
{
    public interface INumberVerificationService
    {
        public Task<BaseResponse> VerifyMobileNumber(string mobileNumber);
    }
}
