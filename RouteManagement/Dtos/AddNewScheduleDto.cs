namespace RouteManagement.Dtos
{
    public class AddNewScheduleDto
    {
        public string routeId {  get; set; }

        public string busId { get; set; }

        public string driverId { get; set; }

        public string departureTime { get; set; }

        public string arrivalTime { get; set; }

        public float price { get; set; }
        
        public string ticketType {  get; set; }
    }
}
