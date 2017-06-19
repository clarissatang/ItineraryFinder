using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace ItineraryFinder
{
    public class CollectError
    {
        // Write all the exceptions to file
        public static void CollectErrorToFile(Exception ex, string errorFile)
        {
            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            File.AppendAllText(errorFile, "Exception : EXE Date " + fileInfo.LastWriteTime.ToString("yyyy/MM/dd H:mm:ss ") + Environment.NewLine);
            File.AppendAllText(errorFile, ex.Message + Environment.NewLine); // Writes : Stack empty.
            File.AppendAllText(errorFile, ex.StackTrace + Environment.NewLine + Environment.NewLine); // Writes : Stack empty.
        }
    }
}
