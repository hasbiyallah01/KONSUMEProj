using System.ComponentModel.DataAnnotations.Schema;

namespace DaticianProj.Core.Domain.Entities
{
    public class Auditables
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Column(TypeName = "timestamp with time zone")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        [Column(TypeName = "timestamp with time zone")]
        public DateTime? DateModified {  get; set; }
    }

}
