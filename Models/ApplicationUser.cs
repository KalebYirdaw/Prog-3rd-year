using Microsoft.AspNetCore.Identity;

namespace GLMS.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? FirstName { get; set; }

        [PersonalData]
        public string? LastName { get; set; }

        [PersonalData]
        public string? UserRole { get; set; } // "Admin" or "Client"

        [PersonalData]
        public int? ClientId { get; set; } // Link to client if role is Client

        [PersonalData]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [PersonalData]
        public bool IsActive { get; set; } = true;

        // Navigation property
        [PersonalData]
        public virtual Client? Client { get; set; }
    }
}