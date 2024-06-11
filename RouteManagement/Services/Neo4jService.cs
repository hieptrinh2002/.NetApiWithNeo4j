using Neo4j.Driver;
using Neo4jClient.Cypher;
using RouteManagement.Models;
using System.Text;
using System.Xml.Linq;

namespace RouteManagement.Services
{
    public class Neo4jService : IDisposable
    {
        private readonly IDriver _driver;

        public Neo4jService(IConfiguration configuration)
        {
            var uri = configuration["Neo4j:Uri"];
            var user = configuration["Neo4j:User"];
            var password = configuration["Neo4j:Password"];
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));

        }

        public async Task<List<City>> GetCitiesAsync()
        {
            var cities = new List<City>();
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync("MATCH (c:City) RETURN c.id AS id, c.name AS name, c.region AS region");
                await result.ForEachAsync(record =>
                {
                    cities.Add(new City
                    {
                        id = record["id"].As<int>(),
                        name = record["name"].As<string>(),
                        region = record["region"].As<string>()
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return cities;
        }

        public async Task<List<_Route>> GetRoutesFromCityAsync(string cityName)
        {
            var routes = new List<_Route>();
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (:City {name: $cityName})-[:HAS_ROUTE]->(r:Route)
                    RETURN r.id AS id, r.name AS name, r.distance AS distance, r.duration AS duration";


                var result = await session.RunAsync(query, new { cityName });
                var te = result.FetchAsync();
                while (await result.FetchAsync())
                {
                    routes.Add(new _Route
                    {
                        id = result.Current["id"].As<string>(),
                        name = result.Current["name"].As<string>(),
                        distance = result.Current["distance"].As<int>(),
                        duration = result.Current["duration"].As<string>()
                    });
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return routes;
        }

        public async Task CreateRelationshipAsync(string startNodeId, string endNodeId, string relationshipType,
                                                  Dictionary<string, object>? properties = null)
        {
            var session = _driver.AsyncSession();
            try
            {
                var query = new StringBuilder($"MATCH (a {{ id: $startNodeId }}), (b {{ id: $endNodeId }}) CREATE (a)-[r:{relationshipType}]");

                if (properties != null && properties.Count > 0)
                {
                    var propertiesString = string.Join(", ", properties.Select(kvp => $"{kvp.Key}: ${kvp.Key}"));
                    query.Append($" {{{propertiesString}}}");
                }

                query.Append("->(b)");

                var parameters = new Dictionary<string, object>
                {
                    { "startNodeId", startNodeId },
                    { "endNodeId", endNodeId }
                };

                if (properties != null)
                {
                    foreach (var kvp in properties)
                    {
                        parameters[kvp.Key] = kvp.Value;
                    }
                }

                await session.RunAsync(query.ToString(), parameters);
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        public async Task<List<_Route>> GetRoutesBetweenCitiesAsync(string cityA, string cityB)
        {
            var routes = new List<_Route>();
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (a:City {name: $cityA})-[:HAS_ROUTE]->(r:Route)<-[:HAS_ROUTE]-(b:City {name: $cityB})
                    RETURN r.id AS id, r.name AS name, r.distance AS distance, r.duration AS duration";

                var result = await session.RunAsync(query, new { cityA, cityB });
                await result.ForEachAsync(record =>
                {
                    routes.Add(new _Route
                    {
                        id = record["id"].As<string>(),
                        name = record["name"].As<string>(),
                        distance = record["distance"].As<int>(),
                        duration = record["duration"].As<string>()
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return routes;
        }

        public async Task<List<Stop>> GetStopsOnRouteAsync(string routeId)
        {
            var stops = new List<Stop>();
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (:Route {id: $routeId})-[:HAS_STOP]->(s:Stop)
                    RETURN s.id AS id, s.name AS name, s.location AS location";
                var result = await session.RunAsync(query, new { routeId });
                await result.ForEachAsync(record =>
                {
                    stops.Add(new Stop
                    {
                        id = record["id"].As<string>(),
                        name = record["name"].As<string>(),
                        location = record["location"].As<string>()
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return stops;
        }

        public async Task<List<_Route>> GetRoutesByOperatorAsync(string operatorId)
        {
            var routes = new List<_Route>();
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (:BusOperator {id: $operatorId})-[:OPERATES]->(r:Route)
                    RETURN r.id AS id, r.name AS name, r.distance AS distance, r.duration AS duration";
                var result = await session.RunAsync(query, new { operatorId });
                await result.ForEachAsync(record =>
                {
                    routes.Add(new _Route
                    {
                        id = record["id"].As<string>(),
                        name = record["name"].As<string>(),
                        distance = record["distance"].As<int>(),
                        duration = record["duration"].As<string>()
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return routes;
        }

        public async Task<List<Schedule>> GetSchedulesByRouteAsync(string routeId)
        {
            var schedules = new List<Schedule>();
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (:Route {id: $routeId})-[:HAS_SCHEDULE]->(s:Schedule)
                    RETURN s.id AS id, s.departureTime AS departureTime, s.arrivalTime AS arrivalTime, s.status AS status";
                var result = await session.RunAsync(query, new { routeId });
                await result.ForEachAsync(record =>
                {
                    schedules.Add(new Schedule
                    {
                        id = record["id"].As<string>(),
                        departureTime = record["departureTime"].As<string>(),
                        arrivalTime = record["arrivalTime"].As<string>(),
                        status = record["status"].As<string>()
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return schedules;
        }

        public async Task<List<Bus>> GetBusesByScheduleAsync(string scheduleId)
        {
            var buses = new List<Bus>();
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (:Schedule {id: $scheduleId})-[:ASSIGNED_TO]->(b:Bus)
                    RETURN b.id AS id, b.licensePlate AS licensePlate, b.capacity AS capacity, b.type AS type";
                var result = await session.RunAsync(query, new { scheduleId });
                await result.ForEachAsync(record =>
                {
                    buses.Add(new Bus
                    {
                        id = record["id"].As<int>(),
                        licensePlate = record["licensePlate"].As<string>(),
                        capacity = record["capacity"].As<int>(),
                        type = record["type"].As<string>()
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return buses;
        }

        public async Task<List<Driver>> GetDriversByScheduleAsync(string scheduleId)
        {
            var drivers = new List<Driver>();
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (:Schedule {id: $scheduleId})-[:DRIVEN_BY]->(d:Driver)
                    RETURN d.id AS id, d.name AS name, d.licenseNumber AS licenseNumber, d.experience AS experience";
                var result = await session.RunAsync(query, new { scheduleId });
                await result.ForEachAsync(record =>
                {
                    drivers.Add(new Driver
                    {
                        id = record["id"].As<int>(),
                        name = record["name"].As<string>(),
                        licenseNumber = record["licenseNumber"].As<string>(),
                        experience = record["experience"].As<int>()
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return drivers;
        }

        public async Task<List<Schedule>> GetSchedulesByDriverAsync(string driverId)
        {
            var schedules = new List<Schedule>();
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (:Driver {id: $driverId})-[:DRIVEN_BY]->(s:Schedule)
                    RETURN s.id AS id, s.departureTime AS departureTime, s.arrivalTime AS arrivalTime, s.status AS status";
                var result = await session.RunAsync(query, new { driverId });
                await result.ForEachAsync(record =>
                {
                    schedules.Add(new Schedule
                    {
                        id = record["id"].As<string>(),
                        departureTime = record["departureTime"].As<string>(),
                        arrivalTime = record["arrivalTime"].As<string>(),
                        status = record["status"].As<string>()
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return schedules;
        }

        public async Task<List<Bus>> GetBusesByOperatorAsync(string operatorId)
        {
            var buses = new List<Bus>();
            var session = _driver.AsyncSession();
            try
            {
                var query = @"
                    MATCH (:BusOperator {id: $operatorId})-[:OPERATES]->(b:Bus)
                    RETURN b.id AS id, b.licensePlate AS licensePlate, b.capacity AS capacity, b.type AS type";
                var result = await session.RunAsync(query, new { operatorId });
                await result.ForEachAsync(record =>
                {
                    buses.Add(new Bus
                    {
                        id = record["id"].As<int>(),
                        licensePlate = record["licensePlate"].As<string>(),
                        capacity = record["capacity"].As<int>(),
                        type = record["type"].As<string>()
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
            return buses;
        }


        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}

//Find all routes from a specific city:


//curl -X GET "https://localhost:5001/Route/fromCity?cityName=CityName"
//Find all stops on a specific route:


//curl -X GET "https://localhost:5001/Stop/onRoute?routeId=RouteId"
//Find the routes that a specific operator operates:

//curl -X GET "https://localhost:5001/Route/byOperator?operatorId=OperatorId"
//Find the schedule for a specific route:

//curl -X GET "https://localhost:5001/Schedule/byRoute?routeId=RouteId"
//Find which buses are assigned to a specific schedule:


//curl -X GET "https://localhost:5001/Bus/bySchedule?scheduleId=ScheduleId"
//Find which drivers are assigned to a particular schedule:


//curl -X GET "https://localhost:5001/Driver/bySchedule?scheduleId=ScheduleId"
//Find all routes between two specific cities:

//curl -X GET "https://localhost:5001/Route/betweenCities?cityA=CityA&cityB=CityB"
//Find the schedules a particular driver is operating:


//curl -X GET "https://localhost:5001/Schedule/byDriver?driverId=DriverId"
//Find all buses operated by a specific bus operator:

//curl -X GET "https://localhost:5001/Bus/byOperator?operatorId=OperatorId"
