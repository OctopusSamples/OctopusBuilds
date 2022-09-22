using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Octopus.Trident.Web.Core.Configuration;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;

namespace Octopus.Trident.Web.DataAccess
{
    public interface IReleaseRepository : IBaseOctopusRepository<ReleaseModel>
    {
        Task<PagedViewModel<ReleaseModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc, int projectId);
    }

    public class ReleaseRepository : BaseRepository<ReleaseModel>, IReleaseRepository
    {
        public ReleaseRepository(IMetricConfiguration metricConfiguration) : base(metricConfiguration)
        {
        }

        public Task<PagedViewModel<ReleaseModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc, int projectId)
        {
            return base.GetAllAsync(currentPageNumber, rowsPerPage, sortColumn, isAsc, $"Where ProjectId = {projectId}");
        }

        public async Task<ReleaseModel> GetByOctopusIdAsync(string octopusId, int parentId)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                var results = await connection.GetListAsync<ReleaseModel>(new { OctopusId = octopusId, projectId = parentId });

                return results.FirstOrDefault();
            }
        }
    }
}