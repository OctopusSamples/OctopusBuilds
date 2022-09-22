using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Octopus.Trident.Web.Core.Configuration;
using Octopus.Trident.Web.Core.Models.ViewModels;

namespace Octopus.Trident.Web.DataAccess
{
    public interface IReportingRepository
    {
        Task<ReportResponseViewModel> GetDeploymentCountsAsync(ReportRequestViewModel request);
        Task<ReportResponseViewModel> GetProjectDeploymentCountsAsync(ReportRequestViewModel request);
        Task<ReportResponseViewModel> GetEnvironmentDeploymentCountsAsync(ReportRequestViewModel request);
    }

    public class ReportingRepository : IReportingRepository
    {
        protected readonly IMetricConfiguration MetricConfiguration;

        public ReportingRepository(IMetricConfiguration metricConfiguration)
        {
            MetricConfiguration = metricConfiguration;
        }

        public async Task<ReportResponseViewModel> GetDeploymentCountsAsync(ReportRequestViewModel request)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                return new ReportResponseViewModel
                {
                    Data = await connection.QueryAsync<ReportResponseDataViewModel>(
                        @"SELECT COUNT(*) AS [Count],
	                               CONVERT(VARCHAR, d.QueueTime, 1) Label
                            FROM dbo.Deployment d
	                            INNER JOIN dbo.Environment e
		                            ON d.EnvironmentId = e.Id
	                            INNER JOIN dbo.Release r
		                            ON d.ReleaseId = r.Id
	                            INNER JOIN dbo.Project p
		                            ON r.ProjectId = p.Id
	                            INNER JOIN dbo.Space s
		                            ON p.SpaceId = s.Id
	                            LEFT JOIN dbo.tenant t
		                            ON d.TenantId = t.Id
                            WHERE d.StartTime BETWEEN @StartDate AND @EndDate
                                and Isnull(@spaceId, s.id) = s.Id
                            GROUP BY CONVERT(VARCHAR, d.QueueTime, 1)
                            Order by CONVERT(VARCHAR, d.QueueTime, 1) asc",
                        new
                        {
                            StartDate = request.StartDate,
                            EndDate = request.EndDate.AddDays(1).AddSeconds(-1),
                            SpaceId = request.SpaceId <= 0 ? null : (int?)request.SpaceId
                        })
                };
            }
        }

        public async Task<ReportResponseViewModel> GetProjectDeploymentCountsAsync(ReportRequestViewModel request)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                return new ReportResponseViewModel
                {
                    Data = await connection.QueryAsync<ReportResponseDataViewModel>(
                        @"SELECT COUNT(*) AS [Count],
	                                p.Name Label
                            FROM dbo.Deployment d
	                            INNER JOIN dbo.Environment e
		                            ON d.EnvironmentId = e.Id
	                            INNER JOIN dbo.Release r
		                            ON d.ReleaseId = r.Id
	                            INNER JOIN dbo.Project p
		                            ON r.ProjectId = p.Id
	                            INNER JOIN dbo.Space s
		                            ON p.SpaceId = s.Id
	                            LEFT JOIN dbo.tenant t
		                            ON d.TenantId = t.Id
                            WHERE d.StartTime BETWEEN @StartDate AND @EndDate
                                and Isnull(@spaceId, s.id) = s.Id
                            GROUP BY p.Name
	                        ORDER BY COUNT(*) desc",
                        new
                        {
                            StartDate = request.StartDate,
                            EndDate = request.EndDate.AddDays(1).AddSeconds(-1),
                            SpaceId = request.SpaceId <= 0 ? null : (int?)request.SpaceId
                        })
                };
            }
        }

        public async Task<ReportResponseViewModel> GetEnvironmentDeploymentCountsAsync(ReportRequestViewModel request)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                return new ReportResponseViewModel
                {
                    Data = await connection.QueryAsync<ReportResponseDataViewModel>(
                        @"SELECT COUNT(*) AS [Count],
	                                e.Name Label
                            FROM dbo.Deployment d
	                            INNER JOIN dbo.Environment e
		                            ON d.EnvironmentId = e.Id
	                            INNER JOIN dbo.Release r
		                            ON d.ReleaseId = r.Id
	                            INNER JOIN dbo.Project p
		                            ON r.ProjectId = p.Id
	                            INNER JOIN dbo.Space s
		                            ON p.SpaceId = s.Id
	                            LEFT JOIN dbo.tenant t
		                            ON d.TenantId = t.Id
                            WHERE d.StartTime BETWEEN @StartDate AND @EndDate
                                and Isnull(@spaceId, s.id) = s.Id
                            GROUP BY e.Name
	                        ORDER BY COUNT(*) desc",
                        new
                        {
                            StartDate = request.StartDate, 
                            EndDate = request.EndDate.AddDays(1).AddSeconds(-1),
                            SpaceId = request.SpaceId <= 0 ? null : (int?)request.SpaceId
                        })
                };
            }
        }
    }
}
