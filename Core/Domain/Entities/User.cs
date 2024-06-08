using Project.Models.Entities;

namespace DaticianProj.Core.Domain.Entities
{
    public class User : Auditables
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Profile Profile { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<VerificationCode> VerificationCodes { get; set; } = new HashSet<VerificationCode>();
    }
}
