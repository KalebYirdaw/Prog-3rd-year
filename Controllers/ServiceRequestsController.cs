using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GLMS.API.Models;
using GLMS.API.Data;

namespace GLMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _context.ServiceRequests.ToListAsync();
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null)
                return NotFound();
            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceRequest request)
        {
            if (request == null)
                return BadRequest();

            request.CreatedAt = DateTime.Now;
            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = request.ServiceRequestId }, request);
        }
    }
}