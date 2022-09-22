using System;
using Octopus.Trident.Web.Core.Constants;
using Octopus.Trident.Web.Core.Models;

namespace Octopus.Trident.Web.BusinessLogic.Factories
{
    public interface ISyncModelFactory
    {
        SyncModel CreateModel(int instanceId, string instanceName, SyncModel previousSync);
    }

    public class SyncModelFactory : ISyncModelFactory
    {
        public SyncModel CreateModel(int instanceId, string instanceName, SyncModel previousSync)
        {
            return new SyncModel
            {
                InstanceId = instanceId,
                Created = DateTime.Now,
                Name = $"Sync for {instanceName}",
                State = SyncState.Queued,
                SearchStartDate = previousSync?.Started
            };
        }
    }
}
