using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GLMS.Models;
using GLMS.Services;
using GLMS.Factories;

namespace GLMS.Controllers
{
    [Authorize]
    public class ServiceRequestsController : Controller
    {
        private readonly IApiService _apiService;
        private readonly CurrencyService _currencyService;

        public ServiceRequestsController(IApiService apiService, CurrencyService currencyService)
        {
            _apiService = apiService;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var requests = await _apiService.GetServiceRequestsAsync();
                ViewBag.IsAdmin = User.IsInRole("Admin");
                return View(requests ?? new List<ServiceRequest>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading service requests: {ex.Message}";
                return View(new List<ServiceRequest>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var request = await _apiService.GetServiceRequestAsync(id);
                if (request == null)
                    return NotFound();
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading service request details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> Create()
        {
            var contracts = await _apiService.GetContractsAsync();
            ViewBag.Contracts = contracts?.Where(c => c.Status == ContractStatus.Active).ToList() ?? new List<Contract>();
            ViewBag.CurrentRate = await GetCurrentRate();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> Create(int contractId, string description, decimal amountUSD)
        {
            var contracts = await _apiService.GetContractsAsync();
            var contract = contracts?.FirstOrDefault(c => c.ContractId == contractId);

            if (contract == null)
            {
                ModelState.AddModelError("", "Contract not found.");
                ViewBag.Contracts = contracts?.Where(c => c.Status == ContractStatus.Active).ToList() ?? new List<Contract>();
                ViewBag.CurrentRate = await GetCurrentRate();
                return View();
            }

            if (contract.Status != ContractStatus.Active)
            {
                ModelState.AddModelError("", "Cannot create service request: Contract is not Active");
                ViewBag.Contracts = contracts?.Where(c => c.Status == ContractStatus.Active).ToList() ?? new List<Contract>();
                ViewBag.CurrentRate = await GetCurrentRate();
                return View();
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                ModelState.AddModelError("", "Description is required");
                ViewBag.Contracts = contracts?.Where(c => c.Status == ContractStatus.Active).ToList() ?? new List<Contract>();
                ViewBag.CurrentRate = await GetCurrentRate();
                return View();
            }

            if (amountUSD <= 0)
            {
                ModelState.AddModelError("", "Amount must be greater than 0");
                ViewBag.Contracts = contracts?.Where(c => c.Status == ContractStatus.Active).ToList() ?? new List<Contract>();
                ViewBag.CurrentRate = await GetCurrentRate();
                return View();
            }

            try
            {
                // Get exchange rate
                var (rate, convertedAmount) = await _currencyService.GetConversionWithRateAsync(amountUSD);

                // Create service request
                var request = new ServiceRequest
                {
                    ContractId = contractId,
                    Description = description,
                    AmountUSD = amountUSD,
                    AmountZAR = convertedAmount,
                    ExchangeRateUsed = rate,
                    Status = RequestStatus.Pending,
                    CreatedAt = DateTime.Now
                };

                var created = await _apiService.CreateServiceRequestAsync(request);

                TempData["Success"] = $"Service request #{created.ServiceRequestId} created successfully! Amount: ${amountUSD:F2} USD = R{convertedAmount:F2} ZAR";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating service request: {ex.Message}");
                ViewBag.Contracts = contracts?.Where(c => c.Status == ContractStatus.Active).ToList() ?? new List<Contract>();
                ViewBag.CurrentRate = await GetCurrentRate();
                return View();
            }
        }

        // GET: ServiceRequests/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var request = await _apiService.GetServiceRequestAsync(id);
                if (request == null)
                    return NotFound();
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading service request: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ServiceRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, ServiceRequest request)
        {
            if (id != request.ServiceRequestId)
            {
                TempData["Error"] = "Request ID mismatch";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var success = await _apiService.UpdateServiceRequestAsync(request);
                    if (success)
                    {
                        TempData["Success"] = $"Service request #{id} updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Error"] = "Failed to update service request";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error updating service request: {ex.Message}";
                }
            }
            return View(request);
        }

        // GET: ServiceRequests/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var request = await _apiService.GetServiceRequestAsync(id);
                if (request == null)
                    return NotFound();
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading service request for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _apiService.DeleteServiceRequestAsync(id);
                if (success)
                {
                    TempData["Success"] = $"Service request #{id} deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete service request";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting service request: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetExchangeRate()
        {
            var rate = await GetCurrentRate();
            return Json(new { rate = rate });
        }

        private async Task<decimal> GetCurrentRate()
        {
            try
            {
                var (rate, _) = await _currencyService.GetConversionWithRateAsync(1);
                return rate;
            }
            catch
            {
                return 19.00m;
            }
        }
    }
}