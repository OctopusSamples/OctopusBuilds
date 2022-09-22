using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octopus.Trident.Web.Core.Models.ViewModels;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly IInstanceRepository _instanceRepository;
        private readonly ISpaceRepository _spaceRepository;

        public ReportController(IInstanceRepository instanceRepository, ISpaceRepository spaceRepository)
        {
            _instanceRepository = instanceRepository;
            _spaceRepository = spaceRepository;
        }

        public async Task<IActionResult> Index(int instanceId)
        {
            var instanceModel = await _instanceRepository.GetByIdAsync(instanceId);
            var spaceList = await _spaceRepository.GetAllAsync(currentPageNumber: 1, rowsPerPage: int.MaxValue, "Name", true, instanceId);

            var viewModel = new ReportingViewModel
            {
                Instance = instanceModel,
                SpaceList = spaceList.Items
            };

            return View(viewModel);
        }
    }
}