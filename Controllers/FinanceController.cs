using Microsoft.AspNetCore.Mvc;
using FinexaApi.Services.Abstract;

namespace FinexaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService _finansService;

        public FinanceController(IFinanceService finansService)
        {
            _finansService = finansService;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetFinanceData()
        {
            var data = await _finansService.GetFinanceDataAsync();
            return Ok(data);
        }
    }
}