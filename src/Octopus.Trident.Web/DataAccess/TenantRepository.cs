using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Octopus.Trident.Web.Core.Configuration;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;

namespace Octopus.Trident.Web.DataAccess
{
    public interface ITenantRepository : IBaseOctopusRepository<TenantModel>
    {
        Task<PagedViewModel<TenantModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc, int spaceId);
    }

    public class TenantRepository : BaseRepository<TenantModel>, ITenantRepository
    {
        public TenantRepository(IMetricConfiguration metricConfiguration) : base(metricConfiguration)
        {

        }

        public Task<PagedViewModel<TenantModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc, int spaceId)
        {
            return base.GetAllAsync(currentPageNumber, rowsPerPage, sortColumn, isAsc, $"Where SpaceId = {spaceId}");
        }

        public async Task<TenantModel> GetByOctopusIdAsync(string octopusId, int parentId)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                var results = await connection.GetListAsync<TenantModel>(new { OctopusId = octopusId, spaceId = parentId });

                return results.FirstOrDefault();
            }
        }
    }
}