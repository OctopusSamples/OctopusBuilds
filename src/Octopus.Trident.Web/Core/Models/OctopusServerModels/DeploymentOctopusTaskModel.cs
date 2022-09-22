using System;

namespace Octopus.Trident.Web.Core.Models.OctopusServerModels
{
    public class DeploymentOctopusTaskModel : BaseOctopusServerModel
    {
        public DateTime QueueTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public string State { get; set; }
    }
}
