using DaticianProj.Models;

namespace DaticianProj.Core.Application.Interfaces.Services
{
    public interface IMailAddressVerification
    {
        public  Task<BaseResponse> VerifyMailAddress(string emailAddress);
    }
}
