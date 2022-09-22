using System.Threading.Tasks;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.CompositeModels;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.BusinessLogic.Factories
{
    public interface ISyncJobCompositeModelFactory
    {
        Task<SyncJobCompositeModel> MakeSyncJobCompositeModelAsync(SyncModel syncModel);
    }

    public class SyncJobCompositeModelFactory : ISyncJobCompositeModelFactory
    {
        private readonly IInstanceRepository _instanceRepository;

        public SyncJobCompositeModelFactory(IInstanceRepository instanceRepository)
        {
            _instanceRepository = instanceRepository;
        }

        public async Task<SyncJobCompositeModel> MakeSyncJobCompositeModelAsync(SyncModel syncModel)
        {
            var instance = await _instanceRepository.GetByIdAsync(syncModel.InstanceId);

            return new SyncJobCompositeModel
            {
                SyncModel = syncModel,
                InstanceModel = instance
            };
        }
    }
}
