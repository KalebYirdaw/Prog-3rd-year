namespace GLMS.Interfaces
{
    public interface ICurrencyStrategy
    {
        Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency);
        string StrategyName { get; }
    }
}