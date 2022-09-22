using System.Collections.Generic;

namespace Octopus.Trident.Web.Core.Models.OctopusServerModels
{
    public class EventOctopusModel : BaseOctopusServerModel
    {
        public string SpaceId { get; set; }
        public string Message { get; set; }
        public List<string> RelatedDocumentIds { get; set; }
    }
}
