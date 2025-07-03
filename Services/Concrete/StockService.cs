using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;

public class StockService : IStockService
{
    private readonly IMemoryCache _memoryCache;

    public StockService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<List<StockQuoteResponseModel>> GetAllQuotesAsync()
    {
        var result = new List<StockQuoteResponseModel>();

        foreach (var symbol in StockSymbolList.Symbols.Distinct())
        {
            if (_memoryCache.TryGetValue($"finnhub:quote:{symbol}", out StockQuoteResponseModel? quote))
            {
                result.Add(quote);
            }
        }

        return Task.FromResult(result);
    }
}
