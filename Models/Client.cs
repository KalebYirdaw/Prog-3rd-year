using System.ComponentModel.DataAnnotations;

namespace GLMS.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Client name is required")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Client Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact email is required")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string ContactEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region is required")]
        [StringLength(50)]
        public string Region { get; set; } = string.Empty;

        // Navigation property - one Client can have many Contracts
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}