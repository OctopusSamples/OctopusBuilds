using System.Collections.Generic;

namespace Octopus.Trident.Web.Core.Models.OctopusServerModels
{
    public class PagedOctopusModel<T> where T : BaseOctopusServerModel
    {
        public List<T> Items { get; set; } = new List<T>();
        public string ItemType { get; set; }
        public int TotalResults { get; set; }
        public int ItemsPerPage { get; set; }
        public int NumberOfPages { get; set; }
        public int LastPageNumber { get; set; }
    }
}
