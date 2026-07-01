using GLMS.Interfaces;

namespace GLMS.Services
{
    public class CurrencyService
    {
        private ICurrencyStrategy _currentStrategy;

        public CurrencyService(ICurrencyStrategy initialStrategy)
        {
            _currentStrategy = initialStrategy;
        }

        public void SetStrategy(ICurrencyStrategy strategy)
        {
            _currentStrategy = strategy;
        }

        public async Task<decimal> ConvertAsync(decimal amountUSD)
        {
            var rate = await _currentStrategy.GetExchangeRateAsync("USD", "ZAR");
            return amountUSD * rate;
        }

        public async Task<(decimal rate, decimal converted)> GetConversionWithRateAsync(decimal amountUSD)
        {
            var rate = await _currentStrategy.GetExchangeRateAsync("USD", "ZAR");
            return (rate, amountUSD * rate);
        }

        public string GetCurrentStrategyName() => _currentStrategy.StrategyName;
    }
}