using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octopus.Trident.Web.Core.Models.ViewModels;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.Controllers.Api
{
    [ApiController]
    [Route("api/instances/{instanceId}/reports")]
    public class ReportController : ControllerBase
    {
        private readonly IReportingRepository _reportingRepository;

        public ReportController(IReportingRepository reportingRepository)
        {
            _reportingRepository = reportingRepository;
        }

        [HttpPost]
        [Route("deploymentcounts")]
        public Task<ReportResponseViewModel> GetDeploymentCounts(ReportRequestViewModel request)
        {
            return _reportingRepository.GetDeploymentCountsAsync(request);
        }

        [HttpPost]
        [Route("projectdeploycounts")]
        public Task<ReportResponseViewModel> GetProjectDeploymentCounts(ReportRequestViewModel request)
        {
            return _reportingRepository.GetProjectDeploymentCountsAsync(request);
        }

        [HttpPost]
        [Route("environmentdeploycounts")]
        public Task<ReportResponseViewModel> GetEnvironmentDeploymentCounts(ReportRequestViewModel request)
        {
            return _reportingRepository.GetEnvironmentDeploymentCountsAsync(request);
        }
    }
}
