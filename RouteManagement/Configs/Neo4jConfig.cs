namespace RouteManagement.Configs;

public class Neo4jConfig
{
    public string Uri { get; }
    public string User { get; }
    public string Password { get; }

    public Neo4jConfig(IConfiguration configuration)
    {
        Uri = configuration["Neo4j:Uri"];
        User = configuration["Neo4j:User"];
        Password = configuration["Neo4j:Password"];
    }
}
