using System;
using Octopus.Trident.Web.Core.Constants;
using Octopus.Trident.Web.Core.Models;

namespace Octopus.Trident.Web.BusinessLogic.Factories
{
    public interface ISyncLogModelFactory
    {
        SyncLogModel MakeInformationLog(string message, int syncId);
        SyncLogModel MakeErrorLog(string message, int syncId);
        SyncLogModel MakeWarningLog(string message, int syncId);
        SyncLogModel Make(string message, int syncId, string logType);
    }

    public class SyncLogModelFactory : ISyncLogModelFactory
    {
        public SyncLogModel MakeInformationLog(string message, int syncId)
        {
            return Make(message, syncId, LogType.Normal);
        }

        public SyncLogModel MakeErrorLog(string message, int syncId)
        {
            return Make(message, syncId, LogType.Error);
        }

        public SyncLogModel MakeWarningLog(string message, int syncId)
        {
            return Make(message, syncId, LogType.Warning);
        }

        public SyncLogModel Make(string message, int syncId, string logType)
        {
            return new SyncLogModel
            {
                SyncId = syncId,
                Created = DateTime.UtcNow,
                Message = message,
                Type = logType
            };
        }
    }
}
