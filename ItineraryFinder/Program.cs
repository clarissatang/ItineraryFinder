using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace ItineraryFinder
{
    public class Program
    {
        public static string ErrorFile;

        [STAThread]
        private static void Main()
        {
            Console.WriteLine("Please select the flight information file:");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            var ofd = new OpenFileDialog
            {
                Multiselect = false,
                Title = "Open Flight Information",
                Filter = "JSON Document|*.json"
            };
            ofd.ShowDialog();

            var jsonString = File.ReadAllText(ofd.FileName);
            
            ErrorFile = "Error_Report.txt";            
            var allFlight = JsonConvert.DeserializeObject<List<FlightInfo>>(jsonString);
            Console.WriteLine("Please enter your departure airport:");
            var srcAirport = Console.ReadLine().ToUpper();
            Console.WriteLine("Please enter your destination airport:");
            var dstAirport = Console.ReadLine().ToUpper();

            var solution = new FindFastestItinerary(allFlight, srcAirport, dstAirport);
            var fastestRoute = solution.ItineraryFinder();
            solution.PrintItinerary(fastestRoute);
        }
    }
}
