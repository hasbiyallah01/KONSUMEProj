using Project.Models.Entities;
using System.Text.Json.Serialization;

namespace DaticianProj.Core.Domain.Entities
{
    public class User : Auditables
    {
        [JsonInclude]
        public string FirstName { get; set; }
        [JsonInclude]
        public string LastName { get; set; }
        [JsonInclude]
        public string Email { get; set; }
        [JsonInclude]
        public string Password { get; set; }
        [JsonInclude]
        public Profile Profile { get; set; }
        public int RoleId { get; set; }
        [JsonInclude]
        public Role Role { get; set; }
        public ICollection<VerificationCode> VerificationCodes { get; set; } = new HashSet<VerificationCode>();
    }
}
