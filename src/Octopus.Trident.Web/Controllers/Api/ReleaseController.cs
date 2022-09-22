using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.Controllers.Api
{
    [ApiController]
    [Route("api/instances/{instanceId}/spaces/{spaceId}/projects/{projectId}/releases")]
    public class ReleaseController : ControllerBase
    {
        private readonly IReleaseRepository _repository;

        public ReleaseController(IReleaseRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public Task<PagedViewModel<ReleaseModel>> GetAll(int projectId, int currentPage = 1, int rowsPerPage = 10, string sortColumn = "Start", bool isAsc = true)
        {
            return _repository.GetAllAsync(currentPage, rowsPerPage, sortColumn, isAsc, projectId);
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ReleaseModel> GetById(int id)
        {
            return _repository.GetByIdAsync(id);
        }
        
        [HttpPost]        
        public Task<ReleaseModel> Insert(int projectId, ReleaseModel model)
        {
            model.ProjectId = projectId;

            return _repository.InsertAsync(model);
        }
        
        [HttpPut]
        [Route("{id}")]
        public Task<ReleaseModel> Update(int projectId, int id, ReleaseModel model)
        {
            model.ProjectId = projectId;
            model.Id = id;
            return _repository.UpdateAsync(model);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public Task Delete(int projectId, int id)
        {
            return _repository.DeleteAsync(id);
        }
    }
}