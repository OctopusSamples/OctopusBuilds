using System;

namespace Octopus.Trident.Web.Core.Models.ViewModels
{
    public class ReportRequestViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SpaceId { get; set; }
    }
}
