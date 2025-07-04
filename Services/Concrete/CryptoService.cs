using FinexaApi.Models.ResponseModel;
using FinexaApi.Services.Abstract;
using Microsoft.Extensions.Caching.Memory;

public class CryptoService : ICryptoService
{
    private readonly IMemoryCache _memoryCache;
    private const string CacheKey = "Top1000Cryptos";

    // 🛡️ Fallback için static değişken
    private static List<CryptoCurrencyResponseModel> _lastKnownCoins = new();

    public CryptoService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<List<CryptoCurrencyResponseModel>> GetTop1000CryptosAsync()
    {
        if (_memoryCache.TryGetValue(CacheKey, out List<CryptoCurrencyResponseModel> cachedCoins) && cachedCoins?.Any() == true)
        {
            // 📌 Bellekte veri varsa, son bilinen veriyi güncelle ve onu döndür
            _lastKnownCoins = cachedCoins;
            return Task.FromResult(cachedCoins);
        }

        // ❗ Bellekte veri yoksa (null, süresi dolmuş veya boş), fallback veriyle dön
        return Task.FromResult(_lastKnownCoins);
    }
}
