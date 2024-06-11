using Neo4jClient;
namespace RouteManagement.Services;

public class RelationshipService
{
    private readonly IGraphClient _client;

    public RelationshipService(IGraphClient client)
    {
        _client = client;
    }

    //public async Task CreateRelationshipAsync(string startNodeId, string endNodeId, string relationshipType,
    //                                          Dictionary<string, object>? properties = null)
    //{
    //    var query = _client.Cypher
    //        .Match("(a { id: {startNodeId} }), (b { id: {endNodeId} })")
    //        .WithParams(new { startNodeId, endNodeId });

    //    if (properties != null && properties.Count > 0)
    //    {
    //        var props = properties.Select(kvp => $"{kvp.Key}: {kvp.Value}").ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    //        query = query.Create($"(a)-[r:{relationshipType} {{{string.Join(", ", props)}}}]->(b)");
    //    }
    //    else
    //    {
    //        query = query.Create($"(a)-[r:{relationshipType}]->(b)");
    //    }

    //    await query.ExecuteWithoutResultsAsync();
    //}
}
