using Neo4jClient;
using RouteManagement.Models;

namespace RouteManagement.Services;

public class BusService
{
    private readonly IGraphClient _client;

    public BusService(IGraphClient client)
    {
        _client = client;
    }

    public async Task<List<Bus>> GetBusesByScheduleAsync(string scheduleId)
    {
        var query = _client.Cypher
            .Match("(:Schedule {id: {scheduleId}})-[:ASSIGNED_TO]->(b:Bus)")
            .WithParam("scheduleId", scheduleId)
            .Return(b => new Bus
            {
                id = b.As<Bus>().id,
                licensePlate = b.As<Bus>().licensePlate,
                capacity = b.As<Bus>().capacity,
                type = b.As<Bus>().type
            });

        var result = await query.ResultsAsync;
        return result.Select(r => new Bus
        {
            id = r.id,
            licensePlate = r.licensePlate,
            capacity = r.capacity,
            type = r.type
        }).ToList();
    }

    public async Task<List<Bus>> GetBusesByOperatorAsync(string operatorId)
    {
        var query = _client.Cypher
            .Match("(:BusOperator {id: {operatorId}})-[:OPERATES]->(b:Bus)")
            .WithParam("operatorId", operatorId)
            .Return(b => new Bus
            {
                id = b.As<Bus>().id,
                licensePlate = b.As<Bus>().licensePlate,
                capacity = b.As<Bus>().capacity,
                type = b.As<Bus>().type
            });

        var result = await query.ResultsAsync;
        return result.Select(r => new Bus
        {
            id = r.id,
            licensePlate = r.licensePlate,
            capacity = r.capacity,
            type = r.type
        }).ToList();
    }
}
