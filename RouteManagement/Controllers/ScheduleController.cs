using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public ScheduleController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("byRoute")]
        public async Task<IActionResult> GetSchedulesByRoute(string routeId)
        {
            var schedules = await _scheduleService.GetSchedulesByRouteAsync(routeId);
            return Ok(schedules);
        }

        [HttpGet("byDriver")]
        public async Task<IActionResult> GetSchedulesByDriver(string driverId)
        {
            var schedules = await _scheduleService.GetSchedulesByDriverAsync(driverId);
            return Ok(schedules);
        }
    }
}
