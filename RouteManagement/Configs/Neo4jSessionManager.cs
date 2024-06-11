using Neo4j.Driver;

namespace RouteManagement.Configs;

public class Neo4jSessionManager
{
    private readonly IDriver _driver;

    public Neo4jSessionManager(Neo4jConfig config)
    {
        _driver = GraphDatabase.Driver(config.Uri, AuthTokens.Basic(config.User, config.Password));
    }

    public IAsyncSession CreateSession()
    {
        return _driver.AsyncSession();
    }

    public void Dispose()
    {
        _driver?.Dispose();
    }
}
