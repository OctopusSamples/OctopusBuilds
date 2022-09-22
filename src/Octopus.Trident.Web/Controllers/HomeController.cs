using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octopus.Trident.Web.BusinessLogic.Factories;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.ViewModels;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IInstanceRepository _instanceRepository;
        private readonly ISyncModelFactory _syncModelFactory;
        private readonly ISyncRepository _syncRepository;

        public HomeController(ILogger<HomeController> logger, 
            IInstanceRepository instanceRepository,
            ISyncModelFactory syncModelFactory,
            ISyncRepository syncRepository)
        {
            _logger = logger;
            _instanceRepository = instanceRepository;
            _syncModelFactory = syncModelFactory;
            _syncRepository = syncRepository;
        }

        public async Task<IActionResult> Index(int currentPage = 1, int rowsPerPage = 10, string sortColumn = "Name", bool isAsc = true)
        {
            var allInstances = await _instanceRepository.GetAllAsync(currentPage, rowsPerPage, sortColumn, isAsc);        

            return View(allInstances);
        }

        public async Task<IActionResult> StartSync(int id)
        {
            var instance = await _instanceRepository.GetByIdAsync(id);
            var previousSync = await _syncRepository.GetLastSuccessfulSync(id);

            var newSync = _syncModelFactory.CreateModel(id, instance.Name, previousSync);

            await _syncRepository.InsertAsync(newSync);

            return RedirectToAction("Index", "Sync");
        }

        public IActionResult AddInstance()
        {
            var instance = new InstanceModel();

            return View("InstanceMaintenance", instance);
        }

        public async Task<IActionResult> EditInstance(int id)
        {
            var instance = await _instanceRepository.GetByIdAsync(id);

            return View("InstanceMaintenance", instance);
        }

        [HttpPost]
        public async Task<IActionResult> Save(InstanceModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View("InstanceMaintenance", model);
            }

            if (model.Id > 0)
            {
                await _instanceRepository.UpdateAsync(model);
            }
            else
            {
                await _instanceRepository.InsertAsync(model);
            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
