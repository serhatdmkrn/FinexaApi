using Microsoft.AspNetCore.Mvc;

namespace FinexaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Finexa API is alive");
        }
    }
}
