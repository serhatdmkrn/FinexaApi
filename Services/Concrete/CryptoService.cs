using Microsoft.Extensions.Caching.Memory;
using FinexaApi.Models.ResponseModel;
using FinexaApi.Services.Abstract;

public class CryptoService : ICryptoService
{
    private readonly IMemoryCache _memoryCache;
    private const string CacheKey = "Top1000Cryptos";

    public CryptoService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<List<CryptoCurrencyResponseModel>> GetTop1000CryptosAsync()
    {
        if (_memoryCache.TryGetValue(CacheKey, out List<CryptoCurrencyResponseModel> cachedCoins))
        {
            return Task.FromResult(cachedCoins);
        }
        else
        {
            return Task.FromResult(new List<CryptoCurrencyResponseModel>());
        }
    }
}