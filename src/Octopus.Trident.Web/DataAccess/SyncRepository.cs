using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Octopus.Trident.Web.Core.Configuration;
using Octopus.Trident.Web.Core.Constants;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;

namespace Octopus.Trident.Web.DataAccess
{
    public interface ISyncRepository : IBaseRepository<SyncModel>
    {
        Task<PagedViewModel<SyncModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc);
        Task<PagedViewModel<SyncModel>> GetNextRecordsToProcessAsync();
        Task<SyncModel> GetLastSuccessfulSync(int instanceId);
    }

    public class SyncRepository : BaseRepository<SyncModel>, ISyncRepository
    {
        public SyncRepository(IMetricConfiguration metricConfiguration) : base(metricConfiguration)
        {
        }

        public Task<PagedViewModel<SyncModel>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc)
        {
            return GetAllAsync(currentPageNumber, rowsPerPage, sortColumn, isAsc, null);
        }

        public Task<PagedViewModel<SyncModel>> GetNextRecordsToProcessAsync()
        {
            const int rowsPerPage = 5;
            const int currentPageNumber = 1;
            var whereClause = $"Where State = '{SyncState.Queued}' and IsNull(RetryAttempts,0) < 5";
            return GetAllAsync(currentPageNumber, rowsPerPage, "Created", isAsc: true, whereClause);
        }

        public async Task<SyncModel> GetLastSuccessfulSync(int instanceId)
        {
            using (var connection = new SqlConnection(base.MetricConfiguration.ConnectionString))
            {
                return (await connection.GetListPagedAsync<SyncModel>(1, 1, $"Where InstanceId = {instanceId} and state = '{SyncState.Completed}'", "Completed Desc")).FirstOrDefault();
            }
        }
    }
}
