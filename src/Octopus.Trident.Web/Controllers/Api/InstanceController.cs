using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.Controllers.Api
{
    [ApiController]
    [Route("api/instances")]
    public class InstanceController : ControllerBase
    {
        private readonly IInstanceRepository _dataAdapter;

        public InstanceController(IInstanceRepository dataAdapter)
        {
            _dataAdapter = dataAdapter;
        }

        [HttpGet]
        
        public Task<PagedViewModel<InstanceModel>> GetAll(int currentPage = 1, int rowsPerPage = 10, string sortColumn = "Name", bool isAsc = true)
        {
            return _dataAdapter.GetAllAsync(currentPage, rowsPerPage, sortColumn, isAsc);
        }
        
        [HttpGet]
        [Route("{id}")]
        public Task<InstanceModel> GetById(int id)
        {
            return _dataAdapter.GetByIdAsync(id);
        }
        
        [HttpPost]
        public Task<InstanceModel> Insert(InstanceModel model)
        {
            return _dataAdapter.InsertAsync(model);
        }
        
        [HttpPut]
        [Route("{id}")]
        public Task<InstanceModel> Update(InstanceModel model)
        {
            return _dataAdapter.UpdateAsync(model);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public Task Delete(int id)
        {
            return _dataAdapter.DeleteAsync(id);
        }
    }
}