using Microsoft.Extensions.Configuration;

namespace Octopus.Trident.Web.Core.Configuration
{
    public interface IMetricConfiguration
    {
        string ConnectionString { get; set; }
    }

    public class MetricConfiguration : IMetricConfiguration
    {
        public string ConnectionString { get; set; }

        public MetricConfiguration(IConfiguration configuration)
        {
            ConnectionString = configuration["ConnectionStrings:Database"];
        }
    }
}
