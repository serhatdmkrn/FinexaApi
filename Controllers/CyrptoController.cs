using Microsoft.AspNetCore.Mvc;
using FinexaApi.Services.Abstract;

namespace FinexaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;

        public CryptoController(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        [HttpGet("top1000")]
        public async Task<IActionResult> GetTop1000()
        {
            var data = await _cryptoService.GetTop1000CryptosAsync();
            return Ok(data);
        }
    }
}