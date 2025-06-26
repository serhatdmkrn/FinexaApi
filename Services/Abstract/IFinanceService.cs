namespace FinexaApi.Services.Abstract
{
    public interface IFinanceService
    {
        Task<Dictionary<string, object>> GetFinanceDataAsync();
    }
}