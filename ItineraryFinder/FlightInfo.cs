using System;

namespace ItineraryFinder
{
    public class FlightInfo
    {
        public int FlightNumber { get; set; }
        public string SourceAirport { get; set; }
        public string DestinationAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
