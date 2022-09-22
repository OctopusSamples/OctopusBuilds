using System;
using System.Threading.Tasks;
using Octopus.Trident.Web.Core.Configuration;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;

namespace Octopus.Trident.Web.DataAccess
{
    public interface ISyncLogRepository : IBaseRepository<SyncLogModel>
    {
        Task<PagedViewModel<SyncLogModel>> GetAllAsync(int syncId, DateTime lastDateRetrieved);
    }

    public class SyncLogRepository : BaseRepository<SyncLogModel>, ISyncLogRepository
    {
        public SyncLogRepository(IMetricConfiguration metricConfiguration) : base(metricConfiguration)
        {
        }

        public Task<PagedViewModel<SyncLogModel>> GetAllAsync(int syncId, DateTime lastDateRetrieved)
        {
            return base.GetAllAsync(0, int.MaxValue, "Id", isAsc: true, $"Where syncId = {syncId} and Created > '{lastDateRetrieved}'");
        }
    }
}
