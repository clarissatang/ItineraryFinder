using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItineraryFinder
{
    public class FindFastestItinerary
    {
        private List<FlightInfo> allFlightInfo;
        private string sourceAirport;
        private string destinationAirport;
        private HashSet<string> openNode;
        private HashSet<string> closeNode;
        public struct prevItineraryInfo
        {
            public string departureAirport;
            public string arriveAirport;
            public DateTime departureTime;
            public DateTime arriveTime;
            public int flightNumber;
        }

        // constructor
        public FindFastestItinerary(List<FlightInfo> allFlight, string srcAirport, string dstAirport)
        {
            allFlightInfo = allFlight;
            sourceAirport = srcAirport;
            destinationAirport = dstAirport;
            openNode = new HashSet<string>();
            closeNode = new HashSet<string>();
        }

        public List<prevItineraryInfo> ItineraryFinder()
        {
            try
            {
                openNode.Add(sourceAirport);
                List<List<prevItineraryInfo>> allRoute = FinderPartialItinerary();
                if (allRoute.Count == 0)
                    return null;
                int useThisRoute = -1;
                DateTime arriveTime = DateTime.MaxValue;
                for (int i = 0; i < allRoute.Count; i++)
                {
                    if (allRoute[i][allRoute[i].Count - 1].arriveAirport == destinationAirport
                        && arriveTime > allRoute[i][allRoute[i].Count - 1].arriveTime)
                    {
                        arriveTime = allRoute[i][allRoute[i].Count - 1].arriveTime;
                        useThisRoute = i;
                    }
                }
                return allRoute[useThisRoute];
            }
            catch(Exception ex)
            {
                CollectError.CollectErrorToFile(ex, Program.errorFile);
                return null;
            }
        } // end: public List<prevItineraryInfo> ItineraryFinder()

        public List<List<prevItineraryInfo>> FinderPartialItinerary()
        {
            try
            {
                List<List<prevItineraryInfo>> allRoute = new List<List<prevItineraryInfo>>();
                while (openNode.Count != 0)
                {
                    // copy open node set in an array since open node set will be changed during this process
                    string[] openNodeArray = new string[openNode.Count];
                    int j = 0;
                    foreach (string oneAirport in openNode)
                        openNodeArray[j++] = oneAirport;

                    foreach (string currAirport in openNodeArray)
                    {
                        if (currAirport == destinationAirport)
                        {
                            // need to exam all route whether we've already get the fastest one, if not, return here to continue search
                            if (isRouteFastest(allRoute) == true)
                                return allRoute;
                            else
                            {
                                openNode.Remove(currAirport);                                
                                continue;
                            }
                        }
                        else // move the current airport to the close set (can't be the destination airport in the future search)
                        {
                            openNode.Remove(currAirport);
                            closeNode.Add(currAirport);
                        }

                        // get all the possible destination airport from this current airport
                        HashSet<string> nextPossibleAirport = new HashSet<string>();
                        for (int i = 0; i < allFlightInfo.Count; i++)
                            if (currAirport == allFlightInfo[i].sourceAirport)
                                nextPossibleAirport.Add(allFlightInfo[i].destinationAirport);

                        // if there's more than one destination airport, we need to copy the current route for multiple possible routes
                        int needToCopyThisRoute = -1;
                        for (int i = 0; i < allRoute.Count; i++)
                        {
                            if (allRoute[i][allRoute[i].Count - 1].arriveAirport == currAirport)
                            {
                                needToCopyThisRoute = i;
                                break;
                            }
                        }
                        if (needToCopyThisRoute != -1)
                        {
                            for (int i = 0; i < nextPossibleAirport.Count - 1; i++)
                            {
                                List<prevItineraryInfo> cloneList = new List<prevItineraryInfo>(allRoute[needToCopyThisRoute]);
                                allRoute.Add(cloneList);
                            }
                        }

                        // check all the destination airport, if it's a possible route, add it to the route, if not, remove the current route
                        foreach (string nextAirport in nextPossibleAirport)
                        {
                            // get the route we are working with
                            int weAreInThisRoute = -1;
                            for (int i = 0; i < allRoute.Count; i++)
                            {
                                if (allRoute[i][allRoute[i].Count - 1].arriveAirport == currAirport)
                                {
                                    weAreInThisRoute = i;
                                    break;
                                }
                            }
                            // if the destination is in the close set, ignore this route
                            if (closeNode.Contains(nextAirport))
                            {
                                allRoute.Remove(allRoute[weAreInThisRoute]);
                                continue;
                            }
                            // calculate time from here to next airport
                            bool isExist = false;
                            int flightNumber = -1;
                            DateTime arriveTime = DateTime.MaxValue;
                            DateTime departureTime = DateTime.MaxValue;
                            string departureAirport = "";
                            string arriveAirport = "";
                            prevItineraryInfo oneItinerary = new prevItineraryInfo();

                            if (currAirport == sourceAirport) // don't have to check 20 mins interval
                            {
                                for (int i = 0; i < allFlightInfo.Count; i++)
                                {
                                    if (allFlightInfo[i].sourceAirport == currAirport
                                        && allFlightInfo[i].destinationAirport == nextAirport)
                                        if (arriveTime > allFlightInfo[i].arrivalTime)
                                        {
                                            departureAirport = allFlightInfo[i].sourceAirport;
                                            arriveAirport = allFlightInfo[i].destinationAirport;
                                            departureTime = allFlightInfo[i].departureTime;
                                            arriveTime = allFlightInfo[i].arrivalTime;
                                            flightNumber = allFlightInfo[i].flightNumber;
                                            isExist = true;
                                        }
                                }
                            } // end: if (currAirport == sourceAirport)
                            else // have to check 20 mins interval
                            {
                                for (int i = 0; i < allFlightInfo.Count; i++)
                                {
                                    if (allFlightInfo[i].sourceAirport == currAirport
                                        && allFlightInfo[i].destinationAirport == nextAirport
                                        && (allRoute[weAreInThisRoute][allRoute[weAreInThisRoute].Count - 1].arriveTime.AddMinutes(20) <= allFlightInfo[i].departureTime))
                                        if (arriveTime > allFlightInfo[i].arrivalTime)
                                        {
                                            departureAirport = allFlightInfo[i].sourceAirport;
                                            arriveAirport = allFlightInfo[i].destinationAirport;
                                            departureTime = allFlightInfo[i].departureTime;
                                            arriveTime = allFlightInfo[i].arrivalTime;
                                            flightNumber = allFlightInfo[i].flightNumber;
                                            isExist = true;
                                        }
                                }
                            }
                            if (isExist == true)
                            {
                                oneItinerary.departureAirport = departureAirport;
                                oneItinerary.arriveAirport = arriveAirport;
                                oneItinerary.departureTime = departureTime;
                                oneItinerary.arriveTime = arriveTime;
                                oneItinerary.flightNumber = flightNumber;
                            }
                            else
                            {
                                allRoute.Remove(allRoute[weAreInThisRoute]);
                                continue;
                            }
                            if (!openNode.Contains(nextAirport))
                                openNode.Add(nextAirport);

                            List<prevItineraryInfo> oneRoute = new List<prevItineraryInfo>();
                            if (weAreInThisRoute == -1) // for the first flight
                            {
                                if (oneItinerary.flightNumber != 0)
                                    oneRoute.Add(oneItinerary);
                                allRoute.Add(oneRoute);
                            }
                            else
                            {
                                if (oneItinerary.flightNumber != 0)
                                    allRoute[weAreInThisRoute].Add(oneItinerary);
                            }
                        } // end: foreach (string nextAirport in nextPossibleAirport)

                    } // end: foreach(string currAirport in openNode)

                }// end: while (openNode.Count != 0)
                return allRoute;
            }
            catch (Exception ex)
            {
                CollectError.CollectErrorToFile(ex, Program.errorFile);
                return null;
            }
        }// end: public List<List<prevItineraryInfo>> FinderPartialItinerary()

        public bool isRouteFastest(List<List<prevItineraryInfo>> allRoute)
        {
            try
            {
                DateTime destinationAirportArriveTime = DateTime.MaxValue;
                DateTime partialTripArriveTime = DateTime.MaxValue;
                for (int i = 0; i < allRoute.Count; i++)
                {
                    if (allRoute[i][allRoute[i].Count - 1].arriveAirport == destinationAirport)
                    {
                        if (destinationAirportArriveTime > allRoute[i][allRoute[i].Count - 1].arriveTime)
                            destinationAirportArriveTime = allRoute[i][allRoute[i].Count - 1].arriveTime;
                    }
                    else if (partialTripArriveTime > allRoute[i][allRoute[i].Count - 1].arriveTime)
                        partialTripArriveTime = allRoute[i][allRoute[i].Count - 1].arriveTime;
                }
                if (destinationAirportArriveTime <= partialTripArriveTime)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                CollectError.CollectErrorToFile(ex, Program.errorFile);
                return false;
            }
        }// end: public bool isRouteFastest(List<List<prevItineraryInfo>> route)

        public void PrintItinerary(List<prevItineraryInfo> fastestRoute)
        {
            if (fastestRoute == null)
            {
                Console.WriteLine("There is no itinerary available!");
            }
            else
            {
                for (int i = 0; i < fastestRoute.Count; i++)
                {
                    Console.WriteLine("Board flight {0} to depart {1} at {2} and arrive at {3} at {4}",
                        fastestRoute[i].flightNumber, fastestRoute[i].departureAirport, fastestRoute[i].departureTime,
                        fastestRoute[i].arriveAirport, fastestRoute[i].arriveTime);
                }
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        } // end: public void PrintRoute(List<prevItineraryInfo> fastestRoute)
    }
}
