using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS.API.Models
{
    public enum RequestStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled,
        Approved
    }

    public class ServiceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServiceRequestId { get; set; }

        public int ContractId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountUSD { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountZAR { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExchangeRateUsed { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}