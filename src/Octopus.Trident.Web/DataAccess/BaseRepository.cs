using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Octopus.Trident.Web.Core.Configuration;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;

namespace Octopus.Trident.Web.DataAccess
{
    public interface IBaseRepository<T> where T : BaseModel
    {
        Task<T> GetByIdAsync(int id);
        Task<T> InsertAsync(T model);
        Task<T> UpdateAsync(T model);
        Task DeleteAsync(int id);
    }

    public interface IBaseOctopusRepository<T> : IBaseRepository<T> where T : BaseModel
    {
        Task<T> GetByOctopusIdAsync(string octopusId, int parentId);
    }

    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseModel
    {
        protected readonly IMetricConfiguration MetricConfiguration;

        protected BaseRepository(IMetricConfiguration metricConfiguration)
        {
            MetricConfiguration = metricConfiguration;
        }

        protected async Task<PagedViewModel<T>> GetAllAsync(int currentPageNumber, int rowsPerPage, string sortColumn, bool isAsc, string whereClause)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                var totalRecords = await connection.RecordCountAsync<T>(null);
                var results = await connection.GetListPagedAsync<T>(currentPageNumber, rowsPerPage, whereClause, $"{sortColumn} {(isAsc ? "asc" : "desc")}");

                return new PagedViewModel<T>
                {
                    Items = results.ToList(),
                    TotalPages = GetTotalPages(totalRecords, rowsPerPage),
                    TotalRecords = totalRecords,
                    CurrentPageNumber = currentPageNumber,
                    RowsPerPage = rowsPerPage
                };
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                return await connection.GetAsync<T>(id);
            }
        }

        public async Task<T> InsertAsync(T model)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                var id = await connection.InsertAsync(model);

                model.Id = id.GetValueOrDefault();

                return model;
            }
        }

        public async Task<T> UpdateAsync(T model)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                await connection.UpdateAsync(model);

                return model;
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(MetricConfiguration.ConnectionString))
            {
                await connection.DeleteAsync<T>(id);

                return;
            }
        }

        protected int GetTotalPages(int totalRecords, int rowsPerPage)
        {
            return Convert.ToInt32(Math.Ceiling((double)totalRecords) / rowsPerPage);
        }
    }
}
