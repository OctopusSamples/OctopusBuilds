using System.ComponentModel.DataAnnotations.Schema;

namespace Octopus.Trident.Web.Core.Models
{
    [Table("Project")]
    public class ProjectModel : BaseOctopusModel
    {
        public int SpaceId { get; set; }
        public string Name { get; set; }
    }
}
