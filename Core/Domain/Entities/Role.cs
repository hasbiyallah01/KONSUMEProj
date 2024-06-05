using DaticianProj.Core.Domain.Entities;

namespace DaticianProj.Core.Domain.Entities
{
    public class Role : Auditables
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
