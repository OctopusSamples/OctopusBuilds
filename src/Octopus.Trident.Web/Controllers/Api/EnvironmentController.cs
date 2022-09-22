using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.Controllers.Api
{
    [ApiController]
    [Route("api/instances/{instanceId}/spaces/{spaceId}/environments")]
    public class EnvironmentController : ControllerBase
    {
        private readonly IEnvironmentRepository _repository;

        public EnvironmentController(IEnvironmentRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]        
        public Task<PagedViewModel<EnvironmentModel>> GetAll(int spaceId, int currentPage = 1, int rowsPerPage = 10, string sortColumn = "Start", bool isAsc = true)
        {
            return _repository.GetAllAsync(currentPage, rowsPerPage, sortColumn, isAsc, spaceId);
        }

        [HttpGet]
        [Route("{id}")]
        public Task<EnvironmentModel> GetById(int id)
        {
            return _repository.GetByIdAsync(id);
        }
        
        [HttpPost]        
        public Task<EnvironmentModel> Insert(int spaceId, EnvironmentModel model)
        {
            model.SpaceId = spaceId;

            return _repository.InsertAsync(model);
        }
        
        [HttpPut]
        [Route("{id}")]
        public Task<EnvironmentModel> Update(int spaceId, int id, EnvironmentModel model)
        {
            model.SpaceId = spaceId;
            model.Id = id;
            return _repository.UpdateAsync(model);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public Task Delete(int id)
        {
            return _repository.DeleteAsync(id);
        }
    }
}