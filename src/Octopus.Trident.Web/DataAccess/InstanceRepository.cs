using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Octopus.Trident.Web.Core.Configuration;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;

namespace Octopus.Trident.Web.DataAccess
{
    public interface IInstanceRepository : IBaseOctopusRepository<InstanceModel>
    {
        Task<PagedViewModel<InstanceModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc);
    }

    public class InstanceRepository : BaseRepository<InstanceModel>, IInstanceRepository
    {
        public InstanceRepository(IMetricConfiguration metricConfiguration) : base(metricConfiguration)
        {
        }

        public Task<PagedViewModel<InstanceModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc)
        {
            return base.GetAllAsync(currentPageNumber, rowsPerPage, sortColumn, isAsc, null);
        }

        public async Task<InstanceModel> GetByOctopusIdAsync(string octopusId, int parentId)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                var results = await connection.GetListAsync<InstanceModel>(new { OctopusId = octopusId });

                return results.FirstOrDefault();
            }
        }
    }
}