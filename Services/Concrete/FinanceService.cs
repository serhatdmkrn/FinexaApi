using FinexaApi.Services.Abstract;
using System.Text.Json;

namespace FinexaApi.Services.Concrete
{
    public class FinanceService : IFinanceService
    {
        private readonly HttpClient _httpClient;

        public FinanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, object>> GetFinanceDataAsync()
        {
            var response = await _httpClient.GetAsync("https://finans.truncgil.com/today.json");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Veri alınamadı.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            return data;
        }
    }
}
