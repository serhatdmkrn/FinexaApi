using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _stockService.GetAllQuotesAsync();
        return Ok(data);
    }
}
