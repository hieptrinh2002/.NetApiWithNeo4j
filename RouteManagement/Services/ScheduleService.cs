using Neo4jClient;
using RouteManagement.Models;

namespace RouteManagement.Services;
public class ScheduleService
{
    private readonly IGraphClient _client;

    public ScheduleService(IGraphClient client)
    {
        _client = client;
    }

    public async Task<List<Schedule>> GetSchedulesByRouteAsync(string routeId)
    {
        var query = _client.Cypher
            .Match("(:Route {id: $routeId})-[:HAS_SCHEDULE]->(s:Schedule)")
            .WithParam("routeId", routeId)
            .Return(s => new Schedule
            {
                id = s.As<Schedule>().id,
                departureTime = s.As<Schedule>().departureTime,
                arrivalTime = s.As<Schedule>().arrivalTime,
                status = s.As<Schedule>().status
            });

        var result = await query.ResultsAsync;
        return result.Select(r => new Schedule
        {
            id = r.id,
            departureTime = r.departureTime,
            arrivalTime = r.arrivalTime,
            status = r.status
        }).ToList();
    }

    public async Task<List<Schedule>> GetSchedulesByDriverAsync(string driverId)
    {
        try
        {
            var query = _client.Cypher
            .Match("(:Driver {id: $driverId})-[:DRIVEN_BY]->(s:Schedule)")
            .WithParam("driverId", driverId)
            .Return(s => new Schedule
            {
                id = s.As<Schedule>().id,
                departureTime = s.As<Schedule>().departureTime,
                arrivalTime = s.As<Schedule>().arrivalTime,
                status = s.As<Schedule>().status
            });

            var result = await query.ResultsAsync;
            return result.Select(r => new Schedule
            {
                id = r.id,
                departureTime = r.departureTime,
                arrivalTime = r.arrivalTime,
                status = r.status
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return new List<Schedule> { new Schedule { } };
        }
      
    }
}
