using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Octopus.Trident.Web.Core.Models
{
    [Table("Release")]
    public class ReleaseModel : BaseOctopusModel
    {
        public int ProjectId { get; set; }
        public string Version { get; set; }
        public DateTime Created { get; set; }
    }
}
