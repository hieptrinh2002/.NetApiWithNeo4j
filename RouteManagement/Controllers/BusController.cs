using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly BusService _busService;

        public BusController(BusService busService)
        {
            _busService = busService;
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
