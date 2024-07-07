using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Neo4jClient;
using Neo4jClient.Cypher;
using RouteManagement.Models;
namespace RouteManagement.Services;

public class RouteService
{
    private readonly IGraphClient _client;
    private readonly IDriver _driver;

    public RouteService(IGraphClient client, IConfiguration configuration)
    {
        _client = client;

        var uri = configuration["Neo4j:Uri"];
        var user = configuration["Neo4j:User"];
        var password = configuration["Neo4j:Password"];
        _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
    }

    public async Task<List<_Route>> GetRoutesFromCityAsync(string cityName)
    {
        try
        {
            var query = _client.Cypher
            .Match("(city:City {name: $cityName})-[:HAS_ROUTE]->(r:Route)")
            .WithParam("cityName", cityName)
            .Return(r => new _Route
            {
                id = r.As<_Route>().id,
                name = r.As<_Route>().name,
                distance = r.As<_Route>().distance,
                duration = r.As<_Route>().duration
            });

            var result = await query.ResultsAsync;

            return result.Select(r => new _Route
            {
                id = r.id,
                name = r.name,
                distance = r.distance,
                duration = r.duration
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return new List<_Route>();
        }
    }

    public async Task<List<_Route>> GetRoutesBetweenCitiesAsync(string cityA, string cityB)
    {
        var query = _client.Cypher
            .Match("(a:City {name: $cityA})-[:HAS_ROUTE]->(r:Route)<-[:HAS_ROUTE]-(b:City {name: $cityB})")
            .WithParams(new { cityA, cityB })
            .Return(r => new _Route
            {
                id = r.As<_Route>().id,
                name = r.As<_Route>().name,
                distance = r.As<_Route>().distance,
                duration = r.As<_Route>().duration
            });

        var result = await query.ResultsAsync;
        return result.Select(r => new _Route
        {
            id = r.id,
            name = r.name,
            distance = r.distance,
            duration = r.duration
        }).ToList();
    }

    public async Task<List<_Route>> GetAllRoutes()
    {
        var query = _client.Cypher
            .Match("(r:Route)")
            .Return(r => new _Route
            {
                id = r.As<_Route>().id,
                name = r.As<_Route>().name,
                distance = r.As<_Route>().distance,
                duration = r.As<_Route>().duration
            });

        var result = await query.ResultsAsync;
        return result.Select(r => new _Route
        {
            id = r.id,
            name = r.name,
            distance = r.distance,
            duration = r.duration
        }).ToList();
    }

}
