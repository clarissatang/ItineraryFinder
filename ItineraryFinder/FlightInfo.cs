using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItineraryFinder
{
    public class FlightInfo
    {
        public int flightNumber { get; set; }
        public string sourceAirport { get; set; }
        public string destinationAirport { get; set; }
        public DateTime departureTime { get; set; }
        public DateTime arrivalTime { get; set; }
    }
}
