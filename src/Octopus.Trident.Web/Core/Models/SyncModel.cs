using System;
using Dapper;

namespace Octopus.Trident.Web.Core.Models
{
    [Table("Sync")]
    public class SyncModel : BaseModel
    {
        public int InstanceId { get; set; }
        public string State { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? SearchStartDate { get; set; }
        public int? RetryAttempts { get; set; }
    }
}
