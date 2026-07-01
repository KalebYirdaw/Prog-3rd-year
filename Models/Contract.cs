using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS.Models
{
    public enum ContractStatus
    {
        Draft,
        Active,
        Expired,
        OnHold,
        Approved,
        Cancelled,
       UnderReview
    }
}

namespace GLMS.Models
{
    public enum ServiceLevel
    {
        Standard,
        Premium,
        Hazardous
    }

    public class Contract
    {
        [Key]
        public int ContractId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        [Required]
        public ServiceLevel ServiceLevel { get; set; } = ServiceLevel.Standard;

        // File handling properties
        public string? SignedAgreementPath { get; set; }
        public string? OriginalFileName { get; set; }
        public long? AgreementFileSize { get; set; }
        public DateTime? AgreementUploadedDate { get; set; }
        public DateTime? LastDownloadedDate { get; set; }
        public int DownloadCount { get; set; } = 0;

        // Navigation property
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

        // Helper property
        [NotMapped]
        public bool IsValidForService => Status == ContractStatus.Active && StartDate <= DateTime.Today && EndDate >= DateTime.Today;
    }
}