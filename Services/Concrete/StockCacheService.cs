using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class StockCacheService : BackgroundService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<StockCacheService> _logger;
    private readonly string _apiKey;
    private int _currentIndex = 0;

    public StockCacheService(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory, ILogger<StockCacheService> logger)
    {
        _memoryCache = memoryCache;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiKey = Environment.GetEnvironmentVariable("FINNHUB_KEY") ?? throw new Exception("FINNHUB_KEY environment variable missing");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = _httpClientFactory.CreateClient();

        _logger.LogInformation("StockCacheService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_currentIndex >= StockSymbolList.Symbols.Count)
                    _currentIndex = 0;

                var symbol = StockSymbolList.Symbols[_currentIndex];
                var url = $"https://finnhub.io/api/v1/quote?symbol={symbol}&token={_apiKey}";

                var response = await client.GetAsync(url, stoppingToken);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync(stoppingToken);
                    var data = JsonSerializer.Deserialize<JsonElement>(json);

                    if (data.TryGetProperty("c", out var currentPriceElem) && currentPriceElem.GetDecimal() != 0)
                    {
                        var model = new StockQuoteResponseModel
                        {
                            Symbol = symbol,
                            CurrentPrice = currentPriceElem.GetDecimal(),
                            HighPrice = data.GetProperty("h").GetDecimal(),
                            LowPrice = data.GetProperty("l").GetDecimal(),
                            OpenPrice = data.GetProperty("o").GetDecimal(),
                            PreviousClose = data.GetProperty("pc").GetDecimal(),
                            Timestamp = data.GetProperty("t").GetInt64()
                        };

                        _memoryCache.Set($"finnhub:quote:{symbol}", model, TimeSpan.FromMinutes(5));
                        _logger.LogInformation($"StockCacheService updated: {symbol}");
                    }
                    else
                    {
                        _logger.LogWarning($"StockCacheService: Price is 0 or missing for {symbol}");
                    }
                }
                else
                {
                    _logger.LogWarning($"StockCacheService: Failed to get data for {symbol}, StatusCode: {response.StatusCode}");
                }

                _currentIndex++;

                await Task.Delay(1000, stoppingToken); // 1 saniye bekle, rate limit için önemli
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StockCacheService error");
                await Task.Delay(5000, stoppingToken); // hata durumunda 5 saniye bekle
            }
        }

        _logger.LogInformation("StockCacheService stopped.");
    }
}
