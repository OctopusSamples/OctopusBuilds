using System.Collections.Generic;
using System.Linq;

namespace Octopus.Trident.Web.Core.Models.ViewModels
{
    public class ReportResponseViewModel
    {
        public IEnumerable<ReportResponseDataViewModel> Data { get; set; }
        public IEnumerable<string> Labels => Data.Select(x => x.Label);
        public IEnumerable<int> Values => Data.Select(x => x.Count);
    }
}
