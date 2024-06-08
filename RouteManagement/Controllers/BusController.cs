using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly Neo4jService _neo4jService;

        public BusController(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        [HttpGet("bySchedule")]
        public async Task<IActionResult> GetBusesBySchedule(string scheduleId)
        {
            var buses = await _neo4jService.GetBusesByScheduleAsync(scheduleId);
            return Ok(buses);
        }

        [HttpGet("byOperator")]
        public async Task<IActionResult> GetBusesByOperator(string operatorId)
        {
            var buses = await _neo4jService.GetBusesByOperatorAsync(operatorId);
            return Ok(buses);
        }
    }
}
