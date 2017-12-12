using System;
using System.Collections.Generic;

namespace ItineraryFinder
{
    public class FindFastestItinerary
    {
        private readonly List<FlightInfo> _allFlightInfo;
        private readonly string _sourceAirport;
        private readonly string _destinationAirport;
        private readonly HashSet<string> _openNode;
        private readonly HashSet<string> _closeNode;
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
            _allFlightInfo = allFlight;
            _sourceAirport = srcAirport;
            _destinationAirport = dstAirport;
            _openNode = new HashSet<string>();
            _closeNode = new HashSet<string>();
        }

        public List<prevItineraryInfo> ItineraryFinder()
        {
            try
            {
                _openNode.Add(_sourceAirport);
                var allRoute = FinderPartialItinerary();
                if (allRoute.Count == 0)
                    return null;
                var useThisRoute = -1;
                var arriveTime = DateTime.MaxValue;
                for (var i = 0; i < allRoute.Count; i++)
                {
                    if (allRoute[i][allRoute[i].Count - 1].arriveAirport == _destinationAirport
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
                CollectError.CollectErrorToFile(ex, Program.ErrorFile);
                return null;
            }
        } // end: public List<prevItineraryInfo> ItineraryFinder()

        public List<List<prevItineraryInfo>> FinderPartialItinerary()
        {
            try
            {
                var allRoute = new List<List<prevItineraryInfo>>();
                while (_openNode.Count != 0)
                {
                    // copy open node set in an array since open node set will be changed during this process
                    var openNodeArray = new string[_openNode.Count];
                    var j = 0;
                    foreach (var oneAirport in _openNode)
                        openNodeArray[j++] = oneAirport;

                    foreach (var currAirport in openNodeArray)
                    {
                        if (currAirport == _destinationAirport)
                        {
                            // need to exam all route whether we've already get the fastest one, if not, return here to continue search
                            if (IsRouteFastest(allRoute))
                                return allRoute;

                            _openNode.Remove(currAirport);
                            continue;
                        }
                        _openNode.Remove(currAirport);
                        _closeNode.Add(currAirport);


                        // get all the possible destination airport from this current airport
                        var nextPossibleAirport = new HashSet<string>();
                        foreach (var oneFlightInfo in _allFlightInfo)
                        {
                            if (currAirport == oneFlightInfo.SourceAirport)
                                nextPossibleAirport.Add(oneFlightInfo.DestinationAirport);
                        }

                        // if there's more than one destination airport, we need to copy the current route for multiple possible routes
                        var needToCopyThisRoute = -1;
                        for (var i = 0; i < allRoute.Count; i++)
                        {
                            if (allRoute[i][allRoute[i].Count - 1].arriveAirport == currAirport)
                            {
                                needToCopyThisRoute = i;
                                break;
                            }
                        }
                        if (needToCopyThisRoute != -1)
                        {
                            for (var i = 0; i < nextPossibleAirport.Count - 1; i++)
                            {
                                var cloneList = new List<prevItineraryInfo>(allRoute[needToCopyThisRoute]);
                                allRoute.Add(cloneList);
                            }
                        }

                        // check all the destination airport, if it's a possible route, add it to the route, if not, remove the current route
                        foreach (var nextAirport in nextPossibleAirport)
                        {
                            // get the route we are working with
                            var weAreInThisRoute = -1;
                            for (var i = 0; i < allRoute.Count; i++)
                            {
                                if (allRoute[i][allRoute[i].Count - 1].arriveAirport == currAirport)
                                {
                                    weAreInThisRoute = i;
                                    break;
                                }
                            }
                            // if the destination is in the close set, ignore this route
                            if (_closeNode.Contains(nextAirport))
                            {
                                allRoute.Remove(allRoute[weAreInThisRoute]);
                                continue;
                            }
                            // calculate time from here to next airport
                            var isExist = false;
                            var flightNumber = -1;
                            var arriveTime = DateTime.MaxValue;
                            var departureTime = DateTime.MaxValue;
                            var departureAirport = "";
                            var arriveAirport = "";
                            var oneItinerary = new prevItineraryInfo();

                            if (currAirport == _sourceAirport) // don't have to check 20 mins interval
                            {
                                foreach(var oneFlightInfo in _allFlightInfo)
                                {
                                    if (oneFlightInfo.SourceAirport == currAirport
                                        && oneFlightInfo.DestinationAirport == nextAirport)
                                        if (arriveTime > oneFlightInfo.ArrivalTime)
                                        {
                                            departureAirport = oneFlightInfo.SourceAirport;
                                            arriveAirport = oneFlightInfo.DestinationAirport;
                                            departureTime = oneFlightInfo.DepartureTime;
                                            arriveTime = oneFlightInfo.ArrivalTime;
                                            flightNumber = oneFlightInfo.FlightNumber;
                                            isExist = true;
                                        }
                                }
                            } // end: if (currAirport == sourceAirport)
                            else // have to check 20 mins interval
                            {
                                foreach (var oneFlightInfo in _allFlightInfo)
                                {
                                    if (oneFlightInfo.SourceAirport == currAirport
                                        && oneFlightInfo.DestinationAirport == nextAirport
                                        && (allRoute[weAreInThisRoute][allRoute[weAreInThisRoute].Count - 1].arriveTime
                                                .AddMinutes(20) <= oneFlightInfo.DepartureTime))
                                        if (arriveTime > oneFlightInfo.ArrivalTime)
                                        {
                                            departureAirport = oneFlightInfo.SourceAirport;
                                            arriveAirport = oneFlightInfo.DestinationAirport;
                                            departureTime = oneFlightInfo.DepartureTime;
                                            arriveTime = oneFlightInfo.ArrivalTime;
                                            flightNumber = oneFlightInfo.FlightNumber;
                                            isExist = true;
                                        }
                                }
                            }
                            if (isExist)
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
                            if (!_openNode.Contains(nextAirport))
                                _openNode.Add(nextAirport);

                            var oneRoute = new List<prevItineraryInfo>();
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
                CollectError.CollectErrorToFile(ex, Program.ErrorFile);
                return null;
            }
        }// end: public List<List<prevItineraryInfo>> FinderPartialItinerary()

        public bool IsRouteFastest(List<List<prevItineraryInfo>> allRoute)
        {
            try
            {
                var destinationAirportArriveTime = DateTime.MaxValue;
                var partialTripArriveTime = DateTime.MaxValue;
                //for (int i = 0; i < allRoute.Count; i++)
                foreach(var oneRoute in allRoute)
                {
                    if (oneRoute[oneRoute.Count - 1].arriveAirport == _destinationAirport)
                    {
                        if (destinationAirportArriveTime > oneRoute[oneRoute.Count - 1].arriveTime)
                            destinationAirportArriveTime = oneRoute[oneRoute.Count - 1].arriveTime;
                    }
                    else if (partialTripArriveTime > oneRoute[oneRoute.Count - 1].arriveTime)
                        partialTripArriveTime = oneRoute[oneRoute.Count - 1].arriveTime;
                }
                return destinationAirportArriveTime <= partialTripArriveTime;
            }
            catch (Exception ex)
            {
                CollectError.CollectErrorToFile(ex, Program.ErrorFile);
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
                foreach(var oneRoute in fastestRoute)
                {
                    Console.WriteLine("Board flight {0} to depart {1} at {2} and arrive at {3} at {4}",
                        oneRoute.flightNumber, oneRoute.departureAirport, oneRoute.departureTime,
                        oneRoute.arriveAirport, oneRoute.arriveTime);
                }
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        } // end: public void PrintRoute(List<prevItineraryInfo> fastestRoute)
    }
}
