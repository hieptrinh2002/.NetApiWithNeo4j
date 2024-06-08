using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly Neo4jService _neo4jService;

        public RouteController(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }
        // GET "https://localhost:5001/Route/between?cityA=CityA&cityB=CityB"

        [HttpGet("fromCity")]
        public async Task<IActionResult> GetRoutesFromCity(string cityName)
        {
            var routes = await _neo4jService.GetRoutesFromCityAsync(cityName);
            return Ok(routes);
        }

        [HttpGet("betweenCities")]
        public async Task<IActionResult> GetRoutesBetweenCities(string cityA, string cityB)
        {
            var routes = await _neo4jService.GetRoutesBetweenCitiesAsync(cityA, cityB);
            return Ok(routes);
        }

        [HttpGet("byOperator")]
        public async Task<IActionResult> GetRoutesByOperator(string operatorId)
        {
            var routes = await _neo4jService.GetRoutesByOperatorAsync(operatorId);
            return Ok(routes);
        }

    }
}
