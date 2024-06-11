using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/route")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly RouteService _routeService;

        public RouteController(RouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet("from-city")]
        public async Task<IActionResult> GetRoutesFromCity(string cityName)
        {
            var routes = await _routeService.GetRoutesFromCityAsync(cityName);
            return Ok(routes);
        }

        // GET "https://localhost:5001/Route/between?cityA=CityA&cityB=CityB"

        [HttpGet("between-cities")]
        public async Task<IActionResult> GetRoutesBetweenCities(string cityA, string cityB)
        {
            var routes = await _routeService.GetRoutesBetweenCitiesAsync(cityA, cityB);
            return Ok(routes);
        }
    }
}
