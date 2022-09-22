using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.Controllers.Api
{
    [ApiController]
    [Route("api/instances/{instanceId}/spaces/{spaceId}/projects/{projectId}/releases/{releaseId}/deployments")]
    public class DeploymentController : ControllerBase
    {
        private readonly IDeploymentRepository _repository;

        public DeploymentController(IDeploymentRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]        
        public Task<PagedViewModel<DeploymentModel>> GetAll(int releaseId, int currentPage = 1, int rowsPerPage = 10, string sortColumn = "Start", bool isAsc = true)
        {
            return _repository.GetAllAsync(currentPage, rowsPerPage, sortColumn, isAsc, releaseId);
        }
        
        [HttpGet]
        [Route("{id}")]
        public Task<DeploymentModel> GetById(int id)
        {
            return _repository.GetByIdAsync(id);
        }
        
        [HttpPost]        
        public Task<DeploymentModel> Insert(int releaseId, DeploymentModel model)
        {
            model.ReleaseId = releaseId;

            return _repository.InsertAsync(model);
        }
        
        [HttpPut]
        [Route("{id}")]
        public Task<DeploymentModel> Update(int releaseId, int id, DeploymentModel model)
        {
            model.ReleaseId = releaseId;
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