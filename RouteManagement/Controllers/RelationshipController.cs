using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteManagement.Services;

namespace RouteManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelationshipController : ControllerBase
    {
        private readonly Neo4jService _neo4jService;

        public RelationshipController(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        //curl -X POST "https://localhost:5001/relationship/create?startNodeId=city1&endNodeId=route1&relationshipType=HAS_ROUTE"
        //curl -X POST "https://localhost:5001/relationship/create?startNodeId=route1&endNodeId=stop1&relationshipType=HAS_STOP" -H "Content-Type: application/json" -d "{\"sequence\": 1}"


        [HttpPost("create")]
        public async Task<IActionResult> CreateRelationship(string startNodeId, string endNodeId, string relationshipType, [FromBody] Dictionary<string, object>? properties = null)
        {
            await _neo4jService.CreateRelationshipAsync(startNodeId, endNodeId, relationshipType, properties);
            return Ok();
        }
    }
}
