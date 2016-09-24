using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarthquakeMonitor.GeoJSON
{
    public class Utils
    {
        /// <summary>
        /// Interprets a GeoJSON timestamp (ms since the "epoch").
        /// </summary>
        /// <param name="unixTimeStamp">GeoJSON timestamp</param>
        /// <returns>Equivalent date-time</returns>
        public static DateTime TimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp / 1000.0);
            dtDateTime = dtDateTime.ToLocalTime();
            return dtDateTime;
        }
    }
}
