using RouteManagement.Models;

namespace RouteManagement.Dtos
{
    public class AddRouteWithStopsDto
    {
        public string routeName { get; set; }

        public int distance { get; set; }
        
        public string duration { get; set; }
        
        public List<Stop> stops { get; set; }  
    }
}
