using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly BusService _busService;
        private readonly IDriver _driver;

        public BusController(BusService busService, IConfiguration configuration)
        {
            _busService = busService;

            var uri = configuration["Neo4j:Uri"];
            var user = configuration["Neo4j:User"];
            var password = configuration["Neo4j:Password"];
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }



        [HttpGet("bySchedule")]
        public async Task<IActionResult> GetBusesBySchedule(string scheduleId)
        {
            var buses = await _busService.GetBusesByScheduleAsync(scheduleId);
            return Ok(buses);
        }

        [HttpGet("byOperator")]
        public async Task<IActionResult> GetBusesByOperator(string operatorId)
        {
            var buses = await _busService.GetBusesByOperatorAsync(operatorId);
            return Ok(buses);
        }
    }
}
