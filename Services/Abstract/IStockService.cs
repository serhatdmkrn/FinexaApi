using System.Collections.Generic;
using System.Threading.Tasks;

public interface IStockService
{
    Task<List<StockQuoteResponseModel>> GetAllQuotesAsync();
}
