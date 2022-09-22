namespace Octopus.Trident.Web.Core.Models.OctopusServerModels
{
    public class DeploymentOctopusModel : BaseOctopusServerModel
    {
        public string EnvironmentId { get; set; }
        public string TenantId { get; set; }
        public string Name { get; set; }

        public DeploymentOctopusModelLinks Links { get; set; } = new DeploymentOctopusModelLinks();

        public class DeploymentOctopusModelLinks
        {
            public string Task { get; set; }
        }
    }
}
