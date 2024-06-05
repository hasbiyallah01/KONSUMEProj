using DaticianProj.Core.Domain.Enum;

namespace DaticianProj.Core.Domain.Entities
{
    public class User : Auditables
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public Gender Gender { get; set; } = default!;
        public int Height { get; set; } = default!;
        public int Weight { get; set; } = default!;
        public string UserGoal { get; set; } = default!;

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
