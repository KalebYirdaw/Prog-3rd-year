using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GLMS.API.Models;
using GLMS.API.Data;

namespace GLMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contracts = await _context.Contracts.ToListAsync();
            return Ok(contracts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound(new { message = "Contract not found" });
            return Ok(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contract contract)
        {
            if (contract == null)
                return BadRequest(new { message = "Invalid contract data" });

            if (contract.ClientId <= 0)
                contract.ClientId = 1;

            // Let database auto-generate the ID
            contract.ContractId = 0;

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = contract.ContractId }, contract);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Contract contract)
        {
            if (id != contract.ContractId)
                return BadRequest(new { message = "ID mismatch" });

            var existing = await _context.Contracts.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Contract not found" });

            existing.ClientId = contract.ClientId;
            existing.StartDate = contract.StartDate;
            existing.EndDate = contract.EndDate;
            existing.Status = contract.Status;
            existing.ServiceLevel = contract.ServiceLevel;
            existing.SignedAgreementPath = contract.SignedAgreementPath;
            existing.OriginalFileName = contract.OriginalFileName;
            existing.AgreementFileSize = contract.AgreementFileSize;
            existing.AgreementUploadedDate = contract.AgreementUploadedDate;
            existing.LastDownloadedDate = contract.LastDownloadedDate;
            existing.DownloadCount = contract.DownloadCount;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound(new { message = "Contract not found" });

            if (Enum.TryParse<ContractStatus>(status, true, out var newStatus))
            {
                contract.Status = newStatus;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Status updated", status = contract.Status });
            }
            return BadRequest(new { message = "Invalid status value" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound(new { message = "Contract not found" });

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Contract deleted" });
        }
    }
}