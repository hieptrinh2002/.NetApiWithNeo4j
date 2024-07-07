using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Neo4jClient;
using RouteManagement.Dtos;
using RouteManagement.Models;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/route")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly RouteService _routeService;
        private readonly IDriver _driver;

        public RouteController(RouteService routeService, IConfiguration configuration)
        {
            _routeService = routeService;
            var uri = configuration["Neo4j:Uri"];
            var user = configuration["Neo4j:User"];
            var password = configuration["Neo4j:Password"];
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        [HttpGet("list-all")]
        public async Task<IActionResult> GetAllRoutes()
        {
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (r:Route)
                    RETURN r AS route
                ";

                var result = await session.ReadTransactionAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query);
                    var routes = await cursor.ToListAsync();

                    return routes.Select(record => record["route"].As<INode>().Properties);
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        [HttpGet("list-all-driver")]
        public async Task<IActionResult> GetAllDrivers()
        {
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (r:Driver)
                    RETURN r AS driver
                ";

                var result = await session.ReadTransactionAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query);
                    var routes = await cursor.ToListAsync();

                    return routes.Select(record => record["driver"].As<INode>().Properties);
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        [HttpGet("list-all-buses")]
        public async Task<IActionResult> GetAllBuses()
        {
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (r:Bus)
                    RETURN r AS bus
                ";

                var result = await session.ReadTransactionAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query);
                    var routes = await cursor.ToListAsync();

                    return routes.Select(record => record["bus"].As<INode>().Properties);
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                await session.CloseAsync();
            }
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


        [HttpGet("all-details")]
        public async Task<IActionResult> GetRouteDetails(string routeId)
        {
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (r:Route {id: $routeId})
                    OPTIONAL MATCH (r)-[:HAS_STOP]->(stop:Stop)
                    OPTIONAL MATCH (r)-[:HAS_SCHEDULE]->(sch:Schedule)
                    OPTIONAL MATCH (sch)<-[:ASSIGNED_TO]-(bus:Bus)
                    OPTIONAL MATCH (sch)<-[:DRIVEN_BY]-(driver:Driver)
                    RETURN r AS route,
                           collect(DISTINCT stop) AS stops,
                           collect(DISTINCT sch) AS schedules,
                           collect(DISTINCT bus) AS buses,
                           collect(DISTINCT driver) AS drivers
                ";

                var result = await session.ReadTransactionAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query, new { routeId });
                    var record = await cursor.SingleAsync();

                    var routeDetails = new
                    {
                        Route = record["route"].As<INode>().Properties,
                        Stops = record["stops"].As<List<INode>>().Select(s => s.Properties),
                        Schedules = record["schedules"].As<List<INode>>().Select(s => s.Properties),
                        Buses = record["buses"].As<List<INode>>().Select(b => b.Properties),
                        Drivers = record["drivers"].As<List<INode>>().Select(d => d.Properties)
                    };

                    return routeDetails;
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        [HttpPost("add-new-with-stops")]
        public async Task<IActionResult> AddRouteWithStops(AddRouteWithStopsDto dto)
        {
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                var routeId = Guid.NewGuid().ToString();

                // Bắt đầu transaction
                await session.WriteTransactionAsync(async tx =>
                {
                    // Tạo node Route
                    await tx.RunAsync("CREATE (route:Route {id: $routeId, name: $routeName, distance: $distance, duration: $duration})",
                        new { routeId, dto.routeName, dto.distance, dto.duration });

                    // Tạo node Stop và mối quan hệ HAS_STOP
                    for (int i = 0; i < dto.stops.Count; i++)
                    {
                        var stopId = Guid.NewGuid().ToString();
                        var stop = dto.stops[i];

                        await tx.RunAsync("CREATE (stop:Stop {id: $stopId, name: $name, location: $location, facilities: $facilities})",
                            new { stopId, stop.name, stop.location, stop.facilities });

                        await tx.RunAsync("MATCH (route:Route {id: $routeId}), (stop:Stop {id: $stopId}) " +
                            "CREATE (route)-[:HAS_STOP {sequence: $sequence}]->(stop)",
                            new { routeId, stopId, sequence = i + 1 });
                    }
                });
                return Ok("Tạo mới tuyến đường thành công !");
            }
            catch(Exception ex)
            {
                return BadRequest("Tạo mới tuyến đường thất bại !");
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        [HttpPost("add-new-schedule")]

        public async Task<IActionResult> AddSchedule(AddNewScheduleDto dto)
        {

            IAsyncSession session = _driver.AsyncSession();
            try
            {
                var scheduleId = Guid.NewGuid().ToString();

                // Bắt đầu transaction
                await session.WriteTransactionAsync(async tx =>
                {
                    // Tạo node Schedule
                    await tx.RunAsync("CREATE (schedule:Schedule {id: $scheduleId, departureTime: $departureTime, arrivalTime: $arrivalTime, status: 'not start', price: $price, ticketType: $ticketType})",
                        new { scheduleId, dto.departureTime, dto.arrivalTime, dto.price, dto.ticketType });
                });
                await session.WriteTransactionAsync(async tx =>
                {
                    // Tạo mối quan hệ ASSIGNED_TO giữa Bus và Schedule
                    await tx.RunAsync("MATCH (bus:Bus {id: $busId}), (schedule:Schedule {id: $scheduleId}) " +
                        "CREATE (bus)-[:ASSIGNED_TO]->(schedule)",
                        new { dto.busId, scheduleId });
                });
                await session.WriteTransactionAsync(async tx =>
                {
                    // Tạo mối quan hệ DRIVEN_BY giữa Driver và Schedule
                    await tx.RunAsync("MATCH (driver:Driver {id: $driverId}), (schedule:Schedule {id: $scheduleId}) " +
                        "CREATE (driver)-[:DRIVEN_BY]->(schedule)",
                        new { dto.driverId, scheduleId });
                });
                await session.WriteTransactionAsync(async tx =>
                {
                    // Tạo mối quan hệ HAS_SCHEDULE giữa Route và Schedule
                    await tx.RunAsync("MATCH (route:Route {id: $routeId}), (schedule:Schedule {id: $scheduleId}) " +
                        "CREATE (route)-[:HAS_SCHEDULE]->(schedule)",
                        new { dto.routeId, scheduleId });
                });
                return Ok("Thêm mới lịch trình thành công !");
            }
            catch (Exception ex)
            {
                return BadRequest("Lịch trình thêm mới thất bại !");
            }
            finally
            {
                await session.CloseAsync();
            }
        }

    }
}
