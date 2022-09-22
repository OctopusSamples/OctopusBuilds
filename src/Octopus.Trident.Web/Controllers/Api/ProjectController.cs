using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.Controllers.Api
{
    [ApiController]
    [Route("api/instances/{instanceId}/spaces/{spaceId}/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _repository;

        public ProjectController(IProjectRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]        
        public Task<PagedViewModel<ProjectModel>> GetAll(int spaceId, int currentPage = 1, int rowsPerPage = 10, string sortColumn = "Name", bool isAsc = true)
        {
            return _repository.GetAllAsync(currentPage, rowsPerPage, sortColumn, isAsc, spaceId);
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ProjectModel> GetById(int spaceId, int id)
        {
            return _repository.GetByIdAsync(id);
        }
        
        [HttpPost]        
        public Task<ProjectModel> Insert(int spaceId, ProjectModel model)
        {
            model.SpaceId = spaceId;

            return _repository.InsertAsync(model);
        }
        
        [HttpPut]
        [Route("{id}")]
        public Task<ProjectModel> Update(int spaceId, int id, ProjectModel model)
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