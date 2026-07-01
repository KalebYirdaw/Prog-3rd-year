using GLMS.Models;
using GLMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IApiService _apiService;

        public DashboardController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminDashboard");
            }
            return RedirectToAction("ClientDashboard");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            ViewBag.UserName = User.Identity?.Name ?? "Admin";
            var contracts = await _apiService.GetContractsAsync();

            ViewBag.TotalContracts = contracts.Count;
            ViewBag.ActiveContracts = contracts.Count(c => c.Status == ContractStatus.Active);
            ViewBag.TotalServiceRequests = 0;
            ViewBag.PendingRequests = 0;

            return View();
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ClientDashboard()
        {
            ViewBag.UserName = User.Identity?.Name ?? "Client";
            var contracts = await _apiService.GetContractsAsync();

            ViewBag.MyContracts = contracts;
            ViewBag.HasActiveContract = contracts.Any(c => c.Status == ContractStatus.Active);
            ViewBag.MyRequests = new List<ServiceRequest>();

            return View();
        }
    }
}