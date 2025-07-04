using FinexaApi.Models.ResponseModel;
using FinexaApi.Services.Abstract;
using Microsoft.Extensions.Caching.Memory;

public class CryptoService : ICryptoService
{
    private readonly IMemoryCache _memoryCache;
    private const string CacheKey = "Top1000Cryptos";

    private static List<CryptoCurrencyResponseModel> _lastKnownCoins = new();

    public CryptoService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<List<CryptoCurrencyResponseModel>> GetTop1000CryptosAsync()
    {
        if (_memoryCache.TryGetValue(CacheKey, out List<CryptoCurrencyResponseModel> cachedCoins) && cachedCoins?.Any() == true)
        {
            _lastKnownCoins = cachedCoins;
            return Task.FromResult(cachedCoins);
        }

        return Task.FromResult(_lastKnownCoins);
    }
}
