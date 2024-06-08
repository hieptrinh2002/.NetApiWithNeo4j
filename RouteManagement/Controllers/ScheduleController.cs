using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly Neo4jService _neo4jService;

        public ScheduleController(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        [HttpGet("byRoute")]
        public async Task<IActionResult> GetSchedulesByRoute(string routeId)
        {
            var schedules = await _neo4jService.GetSchedulesByRouteAsync(routeId);
            return Ok(schedules);
        }

        [HttpGet("byDriver")]
        public async Task<IActionResult> GetSchedulesByDriver(string driverId)
        {
            var schedules = await _neo4jService.GetSchedulesByDriverAsync(driverId);
            return Ok(schedules);
        }
    }
}
