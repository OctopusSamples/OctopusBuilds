using System.ComponentModel.DataAnnotations.Schema;

namespace Octopus.Trident.Web.Core.Models
{
    [Table("Environment")]
    public class EnvironmentModel : BaseOctopusModel
    {
        public int SpaceId { get; set; }
        public string Name { get; set; }
    }
}
