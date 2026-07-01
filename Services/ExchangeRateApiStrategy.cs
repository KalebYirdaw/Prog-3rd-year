using GLMS.Interfaces;
using System.Text.Json;

namespace GLMS.Services
{
    public class ExchangeRateApiStrategy : ICurrencyStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public string StrategyName => "ExchangeRateAPI";

        public ExchangeRateApiStrategy(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _baseUrl = "https://api.exchangerate-api.com/v4/latest/USD";
        }

        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var rates = doc.RootElement.GetProperty("rates");

                if (rates.TryGetProperty(toCurrency, out var rate))
                {
                    return rate.GetDecimal();
                }
                return 19.00m;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return 19.00m;
            }
        }
    }
}