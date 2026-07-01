using System.Text;
using System.Text.Json;
using GLMS.Models;

namespace GLMS.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, IConfiguration config, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _apiBaseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7171/";
            _logger = logger;
        }

        // ========== AUTHENTICATION METHODS ==========

        public async Task<bool> RegisterUserAsync(RegisterViewModel model)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}api/auth/register", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return false;
            }
        }

        public async Task<bool> LoginUserAsync(string email, string password)
        {
            try
            {
                var loginData = new { email, password };
                var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var token = doc.RootElement.GetProperty("token").GetString();
                    // Store token for authenticated requests
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return false;
            }
        }

        // ========== CONTRACT METHODS ==========

        public async Task<List<Contract>> GetContractsAsync(string? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var url = $"{_apiBaseUrl}api/contracts";
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
                if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                if (queryParams.Any()) url += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Contract>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Contract>();
                }
                return new List<Contract>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contracts");
                return new List<Contract>();
            }
        }

        public async Task<Contract?> GetContractAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}api/contracts/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Contract>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting contract {id}");
                return null;
            }
        }

        public async Task<Contract> CreateContractAsync(Contract contract)
        {
            var content = new StringContent(JsonSerializer.Serialize(contract), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}api/contracts", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Contract>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? contract;
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create contract: {response.StatusCode} - {error}");
        }

        public async Task<bool> UpdateContractAsync(Contract contract)
        {
            var content = new StringContent(JsonSerializer.Serialize(contract), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}api/contracts/{contract.ContractId}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}api/contracts/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateContractStatusAsync(int id, string status)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { status }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"{_apiBaseUrl}api/contracts/{id}/status", content);
            return response.IsSuccessStatusCode;
        }

        // ========== SERVICE REQUESTS ==========

        public async Task<List<ServiceRequest>> GetServiceRequestsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}api/servicerequests");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<ServiceRequest>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ServiceRequest>();
                }
                return new List<ServiceRequest>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting service requests");
                return new List<ServiceRequest>();
            }
        }

        public async Task<ServiceRequest?> GetServiceRequestAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}api/servicerequests/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ServiceRequest>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting service request {id}");
                return null;
            }
        }

        public async Task<ServiceRequest> CreateServiceRequestAsync(ServiceRequest request)
        {
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}api/servicerequests", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ServiceRequest>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? request;
            }
            throw new Exception($"Failed to create service request: {response.StatusCode}");
        }

        public async Task<bool> UpdateServiceRequestAsync(ServiceRequest request)
        {
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}api/servicerequests/{request.ServiceRequestId}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteServiceRequestAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}api/servicerequests/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateServiceRequestStatusAsync(int id, string status)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { status }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"{_apiBaseUrl}api/servicerequests/{id}/status", content);
            return response.IsSuccessStatusCode;
        }

        // ========== FILE HANDLING ==========

        public async Task<bool> UploadAgreementAsync(int contractId, Stream fileStream, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                content.Add(streamContent, "file", fileName);

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}api/contracts/{contractId}/upload", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading agreement for contract {contractId}");
                return false;
            }
        }

        public async Task<byte[]?> DownloadAgreementAsync(int contractId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}api/contracts/{contractId}/download");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading agreement for contract {contractId}");
                return null;
            }
        }

        public async Task<bool> DeleteAgreementAsync(int contractId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}api/contracts/{contractId}/agreement");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting agreement for contract {contractId}");
                return false;
            }
        }

        // ========== EXCHANGE RATE ==========

        public async Task<decimal> GetExchangeRateAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://api.exchangerate-api.com/v4/latest/USD");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var rates = doc.RootElement.GetProperty("rates");
                    if (rates.TryGetProperty("ZAR", out var zarRate))
                    {
                        return zarRate.GetDecimal();
                    }
                }
                return 19.00m;
            }
            catch
            {
                return 19.00m;
            }
        }

        // ========== DASHBOARD STATS ==========

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            try
            {
                var contracts = await GetContractsAsync();
                var requests = await GetServiceRequestsAsync();

                return new DashboardStats
                {
                    TotalContracts = contracts.Count,
                    ActiveContracts = contracts.Count(c => c.Status == ContractStatus.Active),
                    TotalServiceRequests = requests.Count,
                    PendingRequests = requests.Count(r => r.Status == RequestStatus.Pending)
                };
            }
            catch
            {
                return new DashboardStats();
            }
        }
    }
}