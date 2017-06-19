Requirement:
Giving all the future flight, source airport and destination airport, find the route which can arrive to the destination airport earlest. The connection flight is invalid if interval time is less than 20 mins.


How to run the application:
1. Double click \ItineraryFinder\bin\Debug\ItineraryFinder.exe
2. Follow the instructions, select one from three .json files (futureFlights.json, testData.json, testData_more.json)
3. Enter the departure airport, and return
4. Enter the destination airport, and return

The algorithm for the solution is:

For any active airport, there are two possible status. OPEN means it acts as a candidate destination in the next searching process; CLOSE means it has already expanded to another airport which can't be acted as a destination airport in the future in order to reduce the searching depth.

1. Start seaching process from the departure airport, remove this airport from OPEN set, add it to CLOSE set;
2. Get all possible destination airports from this airport, add all to the OPEN set;
3. If the destination airport can be reached, record the first available flight, if not(e.g. interval time less than 20mins), delete this route.
4. If the departure airport is the final destination airport, check whether this route is earlier than all the other routes, if yes, this is the fastest route, if not, continue the next searching round.
5. Another break point of the searching process is that the OPEN set is empty.