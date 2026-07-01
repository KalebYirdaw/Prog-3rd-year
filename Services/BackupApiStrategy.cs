using GLMS.Interfaces;
using System.Text.Json;

namespace GLMS.Services
{
    public class BackupApiStrategy : ICurrencyStrategy
    {
        private readonly HttpClient _httpClient;

        public string StrategyName => "BackupAPI";

        public BackupApiStrategy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                // Using a free fallback API
                var response = await _httpClient.GetAsync($"https://api.frankfurter.app/latest?from={fromCurrency}&to={toCurrency}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var rate = doc.RootElement.GetProperty("rates").GetProperty(toCurrency).GetDecimal();

                return rate;
            }
            catch
            {
                return 19.00m; // Fallback
            }
        }
    }
}