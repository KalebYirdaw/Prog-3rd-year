using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Models
{
    public class ContractViewModel
    {
        public Contract Contract { get; set; } = new Contract();
        public IFormFile? SignedAgreement { get; set; }
        public List<SelectListItem>? Clients { get; set; }
    }
}