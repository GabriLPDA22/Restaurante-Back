using Microsoft.AspNetCore.Mvc;
using Restaurante.Services;
using System.Threading.Tasks;
using Restaurante.Services.Interfaces;

namespace Restaurante.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorCounterController : ControllerBase
    {
        private readonly IErrorCounterService _errorCounterService;

        public ErrorCounterController(IErrorCounterService errorCounterService)
        {
            _errorCounterService = errorCounterService;
        }

        [HttpGet]
        public async Task<ActionResult<int>> Get()
        {
            var errorCounterValue = await _errorCounterService.GetErrorCounterValue();
            return Ok(errorCounterValue);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] int value)
        {
            await _errorCounterService.UpdateErrorCounterValue(value);
            return NoContent();
        }
    }
}
