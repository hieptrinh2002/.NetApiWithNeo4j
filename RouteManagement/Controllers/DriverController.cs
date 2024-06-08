using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly Neo4jService _neo4jService;

        public DriverController(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        [HttpGet("bySchedule")]
        public async Task<IActionResult> GetDriversBySchedule(string scheduleId)
        {
            var drivers = await _neo4jService.GetDriversByScheduleAsync(scheduleId);
            return Ok(drivers);
        }
    }
}
