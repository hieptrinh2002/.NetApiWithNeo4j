using Neo4jClient;
using RouteManagement.Models;

namespace RouteManagement.Services;

public class StopService
{
    private readonly IGraphClient _client;

    public StopService(IGraphClient client)
    {
        _client = client;
    }

    public async Task<List<Stop>> GetStopsOnRouteAsync(string routeId)
    {
        var query = _client.Cypher
        .Match("(:Route {id: $routeId})-[:HAS_STOP]->(s:Stop)")
        .WithParam("routeId", routeId)
        .Return(s => new Stop
        {
            id = s.As<Stop>().id,
            name = s.As<Stop>().name,
            location = s.As<Stop>().location
        });

        var result = await query.ResultsAsync;
        return result.Select(r => new Stop
        {
            id = r.id,
            name = r.name,
            location = r.location
        }).ToList();

    }
}
