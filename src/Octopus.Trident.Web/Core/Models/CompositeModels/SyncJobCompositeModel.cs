using System.Collections.Generic;

namespace Octopus.Trident.Web.Core.Models.CompositeModels
{
    public class SyncJobCompositeModel
    {
        public SyncModel SyncModel { get; set; }
        public InstanceModel InstanceModel { get; set; }

        public Dictionary<string, EnvironmentModel> EnvironmentDictionary { get; set; } = new Dictionary<string, EnvironmentModel>();
        public Dictionary<string, TenantModel> TenantDictionary { get; set; } = new Dictionary<string, TenantModel>();
        public Dictionary<string, SpaceModel> SpaceDictionary { get; set; } = new Dictionary<string, SpaceModel>();
        public Dictionary<string, ProjectModel> ProjectDictionary { get; set; } = new Dictionary<string, ProjectModel>();
    }
}
