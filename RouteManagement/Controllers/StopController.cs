using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StopController : ControllerBase
    {
        private readonly Neo4jService _neo4jService;

        public StopController(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        [HttpGet("onRoute")]
        public async Task<IActionResult> GetStopsOnRoute(string routeId)
        {
            var stops = await _neo4jService.GetStopsOnRouteAsync(routeId);
            return Ok(stops);
        }
    }
}
