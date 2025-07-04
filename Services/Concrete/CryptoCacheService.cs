using FinexaApi.Models.ResponseModel;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

public class CryptoCacheService : BackgroundService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CryptoCacheService> _logger;

    private const string CacheKey = "Top1000Cryptos";
    private const int CacheDurationSeconds = 90;

    public CryptoCacheService(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory, ILogger<CryptoCacheService> logger)
    {
        _memoryCache = memoryCache;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CryptoCacheService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var allCoins = new List<CryptoCurrencyResponseModel>();

                var client = _httpClientFactory.CreateClient("Crypto");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("FinexaApi/1.0");

                for (int page = 1; page <= 4; page++)
                {
                    var url = $"https://api.coingecko.com/api/v3/coins/markets" +
                              $"?vs_currency=usd&order=market_cap_desc&per_page=250&page={page}";

                    var response = await client.GetAsync(url, stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync(stoppingToken);
                        var coins = JsonSerializer.Deserialize<List<CryptoCurrencyResponseModel>>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (coins != null)
                        {
                            allCoins.AddRange(coins);
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"CoinGecko API failed at page {page} with status {response.StatusCode}");
                    }
                }

                _memoryCache.Set(CacheKey, allCoins, TimeSpan.FromSeconds(100));

                _logger.LogInformation($"Cache updated with {allCoins.Count} cryptocurrencies.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating crypto cache");
            }

            await Task.Delay(TimeSpan.FromSeconds(CacheDurationSeconds), stoppingToken);
        }

        _logger.LogInformation("CryptoCacheService stopped.");
    }
}