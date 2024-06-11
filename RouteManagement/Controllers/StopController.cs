using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StopController : ControllerBase
    {
        private readonly StopService _stopService;

        public StopController(StopService stopService)
        {
            _stopService = stopService;
        }

        [HttpGet("onRoute")]
        public async Task<IActionResult> GetStopsOnRoute(string routeId)
        {
            var stops = await _stopService.GetStopsOnRouteAsync(routeId);
            return Ok(stops);
        }
    }
}
