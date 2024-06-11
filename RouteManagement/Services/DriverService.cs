using Neo4jClient;

namespace RouteManagement.Services;

public class DriverService
{
    private readonly IGraphClient _client;

    public DriverService(IGraphClient client)
    {
        _client = client;
    }
}
