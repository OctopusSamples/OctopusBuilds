using System.ComponentModel.DataAnnotations.Schema;

namespace Octopus.Trident.Web.Core.Models
{
    [Table("Space")]
    public class SpaceModel : BaseOctopusModel
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
    }
}
