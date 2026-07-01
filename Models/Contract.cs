using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS.API.Models
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

    public enum ServiceLevel
    {
        Standard,
        Premium,
        Hazardous
    }

    public class Contract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContractId { get; set; }  // Changed from Id to ContractId

        public int ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Draft;
        public ServiceLevel ServiceLevel { get; set; } = ServiceLevel.Standard;
        public string? SignedAgreementPath { get; set; }
        public string? OriginalFileName { get; set; }
        public long? AgreementFileSize { get; set; }
        public DateTime? AgreementUploadedDate { get; set; }
        public DateTime? LastDownloadedDate { get; set; }
        public int DownloadCount { get; set; } = 0;
    }
}