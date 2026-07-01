using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GLMS.Models;
using GLMS.Services;

namespace GLMS.Controllers
{
    [Authorize]
    public class ContractsController : Controller
    {
        private readonly IApiService _apiService;

        public ContractsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string? status, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var contracts = await _apiService.GetContractsAsync(status, startDate, endDate);
                ViewBag.IsAdmin = User.IsInRole("Admin");
                ViewBag.StatusFilter = status;
                ViewBag.StartDateFilter = startDate?.ToString("yyyy-MM-dd");
                ViewBag.EndDateFilter = endDate?.ToString("yyyy-MM-dd");
                return View(contracts);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading contracts: {ex.Message}";
                return View(new List<Contract>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var contract = await _apiService.GetContractAsync(id);
                if (contract == null) return NotFound();
                return View(contract);
            }
            catch
            {
                TempData["Error"] = "Error loading contract details";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set default values if not provided
                    if (contract.ClientId == 0) contract.ClientId = 1;

                    var created = await _apiService.CreateContractAsync(contract);
                    TempData["Success"] = $"Contract #{created.ContractId} created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating contract: {ex.Message}");
                }
            }
            return View(contract);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var contract = await _apiService.GetContractAsync(id);
                if (contract == null) return NotFound();
                return View(contract);
            }
            catch
            {
                TempData["Error"] = "Error loading contract for editing";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract)
        {
            if (id != contract.ContractId)
            {
                TempData["Error"] = "Contract ID mismatch";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var success = await _apiService.UpdateContractAsync(contract);
                    if (success)
                    {
                        TempData["Success"] = $"Contract #{id} updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Error"] = "Failed to update contract";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error updating contract: {ex.Message}";
                }
            }
            return View(contract);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var contract = await _apiService.GetContractAsync(id);
                if (contract == null) return NotFound();
                return View(contract);
            }
            catch
            {
                TempData["Error"] = "Error loading contract for deletion";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _apiService.DeleteContractAsync(id);
                if (success)
                {
                    TempData["Success"] = $"Contract #{id} deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete contract";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting contract: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}