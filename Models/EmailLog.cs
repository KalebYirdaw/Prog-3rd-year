using System.ComponentModel.DataAnnotations;

namespace GLMS.Models
{
    public class EmailLog
    {
        public string? To;

        [Key]
        public int EmailLogId { get; set; }

        [Required]
        public string? ToEmail { get; set; }

        [Required]
        public string? Subject { get; set; }

        [Required]
        public string? Body { get; set; }

        [Required]
        public DateTime SentAt { get; set; } = DateTime.Now;

        public string? Status { get; set; } = "Sent";

        public string? RelatedEntity { get; set; } // e.g., "Contract", "ServiceRequest"

        public int? RelatedEntityId { get; set; }

        public bool IsRead { get; set; } = false;
    }
}