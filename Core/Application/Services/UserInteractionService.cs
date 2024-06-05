using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Core.Domain.Entities;
using DaticianProj.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace DaticianProj.Core.Application.Services
{

    public class UserInteractionService : IUserInteractionService
    {
        private readonly KonsumeContext _context;

        public UserInteractionService(KonsumeContext context)
        {
            _context = context;
        }

        public async Task<UserInteraction> SaveUserInteractionAsync(string question, string response)
        {
            var interaction = new UserInteraction
            {
                Question = question,
                Response = response,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserInteractions.Add(interaction);
            await _context.SaveChangesAsync();
            return interaction;
        }

        public async Task<List<UserInteraction>> GetUserInteractionsAsync()
        {
            return await _context.UserInteractions.OrderByDescending(ui => ui.CreatedAt).ToListAsync();
        }
    }
}

