using GLMS.Models;

namespace GLMS.Services
{
    public interface IApiService
    {
        // Contracts
        Task<List<Contract>> GetContractsAsync(string? status = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<Contract?> GetContractAsync(int id);
        Task<Contract> CreateContractAsync(Contract contract);
        Task<bool> UpdateContractAsync(Contract contract);
        Task<bool> DeleteContractAsync(int id);
        Task<bool> UpdateContractStatusAsync(int id, string status);

        // Authentication
        Task<bool> RegisterUserAsync(RegisterViewModel model);
        Task<bool> LoginUserAsync(string email, string password);

        // File Handling
        Task<bool> UploadAgreementAsync(int contractId, Stream fileStream, string fileName);
        Task<byte[]?> DownloadAgreementAsync(int contractId);
        Task<bool> DeleteAgreementAsync(int contractId);

        // Service Requests
        Task<List<ServiceRequest>> GetServiceRequestsAsync();
        Task<ServiceRequest?> GetServiceRequestAsync(int id);
        Task<ServiceRequest> CreateServiceRequestAsync(ServiceRequest request);
        Task<bool> UpdateServiceRequestAsync(ServiceRequest request);
        Task<bool> DeleteServiceRequestAsync(int id);
        Task<bool> UpdateServiceRequestStatusAsync(int id, string status);

        // Exchange Rate
        Task<decimal> GetExchangeRateAsync();

        // Dashboard Stats
        Task<DashboardStats> GetDashboardStatsAsync();
    }

    public class DashboardStats
    {
        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public int TotalServiceRequests { get; set; }
        public int PendingRequests { get; set; }
    }
}