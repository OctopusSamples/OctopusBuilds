using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.Controllers
{
    public class SyncController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISyncRepository _syncRepository;

        public SyncController(ILogger<HomeController> logger, ISyncRepository syncRepository)
        {
            _logger = logger;
            _syncRepository = syncRepository;
        }

        public async Task<IActionResult> Index(int currentPage = 1, int rowsPerPage = 10, string sortColumn = "Name", bool isAsc = true)
        {
            var pagedSyncView = await _syncRepository.GetAllAsync(currentPage, rowsPerPage, sortColumn, isAsc);

            return View(pagedSyncView);
        }
    }
}
