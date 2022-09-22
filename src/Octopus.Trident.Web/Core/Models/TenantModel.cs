using System.ComponentModel.DataAnnotations.Schema;

namespace Octopus.Trident.Web.Core.Models
{
    [Table("Tenant")]
    public class TenantModel : BaseOctopusModel
    {
        public int SpaceId { get; set; }
        public string Name { get; set; }
    }
}
