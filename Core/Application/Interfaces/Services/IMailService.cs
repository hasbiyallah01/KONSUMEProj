using DaticianProj.Models;

namespace DaticianProj.Core.Application.Interfaces.Services
{
    public interface IMailService
    {
        public void SendEmail(MailRequest mailRequest);
    }
}
