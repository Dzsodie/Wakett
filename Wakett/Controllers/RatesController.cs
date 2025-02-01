using System;
using Microsoft.AspNetCore.Mvc;
using RatesService.Services;

namespace RatesService.Controllers
{
    [ApiController]
    [Route("api/rates")]
    public class RatesController : ControllerBase
    {
        private readonly RateChangeService _service;

        public RatesController(RateChangeService service)
        {
            _service = service;
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> FetchRates()
        {
            await _service.CheckForRateChangesAsync();
            return Ok("Rates processed successfully.");
        }
    }

}

