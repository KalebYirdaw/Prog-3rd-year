using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS.Models
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
        public int ServiceRequestId { get; set; }

        [Required]
        public int ContractId { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract? Contract { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999.99)]
        [Display(Name = "Amount (USD)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountUSD { get; set; }

        [Display(Name = "Amount (ZAR)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountZAR { get; set; }

        [Display(Name = "Exchange Rate Used")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal ExchangeRateUsed { get; set; }

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}