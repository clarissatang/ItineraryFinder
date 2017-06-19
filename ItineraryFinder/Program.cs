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
    class Program
    {
        public static string errorFile;

        [STAThread]
        static void Main()
        {
            Console.WriteLine("Please select the flight information file:");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Title = "Open Flight Information";
            ofd.Filter = "JSON Document|*.json";
            ofd.ShowDialog();

            string JSONString = File.ReadAllText(ofd.FileName);
            
            errorFile = "Error_Report.txt";            
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<FlightInfo> allFlight = JsonConvert.DeserializeObject<List<FlightInfo>>(JSONString);
            Console.WriteLine("Please enter your departure airport:");
            string srcAirport = Console.ReadLine().ToUpper();
            Console.WriteLine("Please enter your destination airport:");
            string dstAirport = Console.ReadLine().ToUpper();

            FindFastestItinerary solution = new FindFastestItinerary(allFlight, srcAirport, dstAirport);
            List<FindFastestItinerary.prevItineraryInfo> fastestRoute = solution.ItineraryFinder();
            solution.PrintItinerary(fastestRoute);
        }
    }
}
