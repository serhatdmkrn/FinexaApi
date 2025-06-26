using FinexaApi.Models.ResponseModel;

namespace FinexaApi.Services.Abstract
{
    public interface ICryptoService
    {
        Task<List<CryptoCurrencyResponseModel>> GetTop1000CryptosAsync();
    }
}