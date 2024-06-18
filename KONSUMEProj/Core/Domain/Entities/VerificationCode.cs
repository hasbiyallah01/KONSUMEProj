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

    public class EmailConfiguration : Auditables
    {
        public string EmailSenderAddress { get; set; }
        public string EmailSenderName { get; set; }
        public string EmailSenderPassword { get; set; }
        public string SMTPServerAddress { get; set; }
        public int SMTPServerPort { get; set; }
        public bool SMTPServerEnableSSL { get; set; }
    }

}
