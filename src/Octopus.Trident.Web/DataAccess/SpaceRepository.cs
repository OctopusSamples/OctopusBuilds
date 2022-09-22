using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Octopus.Trident.Web.Core.Configuration;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;

namespace Octopus.Trident.Web.DataAccess
{
    public interface ISpaceRepository : IBaseOctopusRepository<SpaceModel>
    {
        Task<PagedViewModel<SpaceModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc, int instanceId);
    }

    public class SpaceRepository : BaseRepository<SpaceModel>, ISpaceRepository
    {
        public SpaceRepository(IMetricConfiguration metricConfiguration) : base(metricConfiguration)
        {
        }

        public Task<PagedViewModel<SpaceModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc, int instanceId)
        {
            return base.GetAllAsync(currentPageNumber, rowsPerPage, sortColumn, isAsc, $"Where InstanceId = {instanceId}");
        }

        public async Task<SpaceModel> GetByOctopusIdAsync(string octopusId, int parentId)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                var results = await connection.GetListAsync<SpaceModel>(new { OctopusId = octopusId, instanceId = parentId });

                return results.FirstOrDefault();
            }
        }
    }
}