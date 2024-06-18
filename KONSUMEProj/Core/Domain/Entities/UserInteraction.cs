using System.ComponentModel.DataAnnotations.Schema;

namespace DaticianProj.Core.Domain.Entities
{
    public class UserInteraction :Auditables
        {
            public string Question { get; set; }
            public string Response { get; set; }
        }

}
