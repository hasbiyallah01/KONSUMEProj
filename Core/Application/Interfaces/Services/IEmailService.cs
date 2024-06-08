using DaticianProj.Core.Domain.Entities;
using DaticianProj.Models;

namespace DaticianProj.Core.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailClient(string msg, string title, string email);
        Task<BaseResponse> SendNotificationToUserAsync(User user);
        Task<bool> SendEmailAsync(MailRecieverDto model, MailRequests request);
    }
}
