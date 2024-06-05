namespace DaticianProj.Core.Domain.Entities
{
    public class Auditables
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; } 
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? DateModified {  get; set; }
    }

}
