using Neo4jClient;
using RouteManagement.Models;

namespace RouteManagement.Services;

public class CityService
{
    private readonly IGraphClient _client;

    public CityService(IGraphClient client)
    {
        _client = client;
    }

    public async Task<List<City>> GetCitiesAsync()
    {
        var query = _client.Cypher
            .Match("(c:City)")
            .Return(c => new City
            {
                id = c.As<City>().id,
                name = c.As<City>().name,
                region = c.As<City>().region
            });

        var result = await query.ResultsAsync;
        return result.Select(r => new City
        {
            id = r.id,
            name = r.name,
            region = r.region
        }).ToList();
    }
}
