using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GLMS.Services;

namespace GLMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IApiService _apiService;

        public AdminController(IEmailService emailService, IApiService apiService)
        {
            _emailService = emailService;
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            // Return empty list for demo (no database)
            var users = new List<GLMS.Models.ApplicationUser>();
            return View(users);
        }

        public async Task<IActionResult> EmailLogs()
        {
            var emails = await _emailService.GetEmailLogsAsync();
            return View(emails);
        }

        [HttpPost]
        public async Task<IActionResult> MarkEmailAsRead(int id)
        {
            await _emailService.MarkEmailAsRead(id);
            return RedirectToAction(nameof(EmailLogs));
        }
    }
}