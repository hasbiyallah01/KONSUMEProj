using DaticianProj.Core.Domain.Entities;

namespace Project.Models.Entities
{
    public class VerificationCode :Auditables
    {
        public int Code { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
