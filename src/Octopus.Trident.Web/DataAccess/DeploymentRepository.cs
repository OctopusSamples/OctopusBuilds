using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Octopus.Trident.Web.Core.Configuration;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;

namespace Octopus.Trident.Web.DataAccess
{
    public interface IDeploymentRepository : IBaseOctopusRepository<DeploymentModel>
    {
        Task<PagedViewModel<DeploymentModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc, int releaseId);
    }

    public class DeploymentRepository : BaseRepository<DeploymentModel>, IDeploymentRepository
    {
        public DeploymentRepository(IMetricConfiguration metricConfiguration) : base(metricConfiguration)
        {
            
        }

        public Task<PagedViewModel<DeploymentModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc, int releaseId)
        {
            return base.GetAllAsync(currentPageNumber, rowsPerPage, sortColumn, isAsc, $"Where ReleaseId = {releaseId}");
        }

        public async Task<DeploymentModel> GetByOctopusIdAsync(string octopusId, int parentId)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                var results = await connection.GetListAsync<DeploymentModel>(new { OctopusId = octopusId, ReleaseId = parentId });

                return results.FirstOrDefault();
            }
        }
    }
}