using DaticianProj.Core.Domain.Entities;
using DaticianProj.Models;

namespace DaticianProj.Core.Application.Interfaces.Services
{
    public interface IUserInteractionService
    {
        Task<UserInteraction> SaveUserInteractionAsync(string question, string response);
        Task<List<UserInteraction>> GetUserInteractionsAsync();
    }

}

