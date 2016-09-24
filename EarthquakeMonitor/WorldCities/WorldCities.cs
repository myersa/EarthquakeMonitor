using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Device.Location;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using EarthquakeMonitor.Properties;

namespace EarthquakeMonitor.WorldCities
{
    /// <summary>
    /// Represents a city
    /// </summary>
    public class City
    {
        public City(string name, float latitude, float longitude)
            : this(name, new GeoCoordinate(latitude, longitude))
        {}

        public City(string name, GeoCoordinate coordinates)
        {
            Name = name;
            Coordinates = coordinates;
        }

        public string Name { get; private set; }
        public GeoCoordinate Coordinates { get; private set; }
    }

    /// <summary>
    /// Combines a city with a distance. Used in finding nearest cities to a given coordinate.
    /// </summary>
    class CityDistance
    {
        public CityDistance(City city, double distance)
        {
            City = city;
            Distance = distance;
        }

        public City City { get; private set; }
        public double Distance { get; private set; }

        public override string ToString()
        {
            return String.Format("{0} ({1:.#} km)", City.Name, Distance / 1000.0);
        }
    }

    /// <summary>
    /// Responsible for data and logic related to world cities.
    /// </summary>
    class WorldCities
    {
        const int COLID_LANG_SCRIPT = 5;
        const int COLID_NAME = 6;
        const int COLID_LAT = 7;
        const int COLID_LONG = 8;

        public List<City> Cities { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public WorldCities()
        {
            Cities = new List<City>();
        }

        /// <summary>
        /// Load cities data from zipped CSV. This can take several seconds.
        /// </summary>
        public void Load()
        {
            Cities.Clear();

            // Create temp folder
            string tempDir = GetTemporaryDirectory();

            string zipPath = Path.Combine(tempDir, Settings.Default.WorldCities_ZipFile);
            File.WriteAllBytes(zipPath, Resources.worldcities_zip);

            // Unzip worldcities ZIP resource into temp directory
            // Should create <tempDir>/worldcities.csv
            ZipFile.ExtractToDirectory(zipPath, tempDir);

            string csvPath = Path.Combine(tempDir, Settings.Default.WorldCities_CsvFile);

            using (TextFieldParser parser = new TextFieldParser(csvPath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // Read headers, assert things are in the right place
                string[] headers = parser.ReadFields();
                AssertHeader(headers, COLID_LANG_SCRIPT, "language script");
                AssertHeader(headers, COLID_NAME, "name");
                AssertHeader(headers, COLID_LAT, "latitude");
                AssertHeader(headers, COLID_LONG, "longitude");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    Cities.Add(new City(
                        fields[COLID_NAME],
                        float.Parse(fields[COLID_LAT]),
                        float.Parse(fields[COLID_LONG])
                    ));
                }
            }

            // Now can delete the temp folder we created
            Directory.Delete(tempDir, true);
        }

        /// <summary>
        /// Gets the nearest cities to a specific coordinate.
        /// </summary>
        /// <param name="coordinate">Coordinate</param>
        /// <param name="k">Number of nearest cities to get.</param>
        /// <returns></returns>
        public List<CityDistance> GetNearestCities(GeoCoordinate coordinate, int k = 3)
        {
            // k nearest cities. Sorted by distance
            // Use a LinkedList for efficient insertion
            LinkedList<CityDistance> cityDistances = new LinkedList<CityDistance>();

            foreach (City city in Cities)
            {
                // Get distance using GeoCoordinate arithmatic
                CityDistance info = new CityDistance(city, coordinate.GetDistanceTo(city.Coordinates));

                if (cityDistances.Count == 0)
                {
                    // List is empty
                    cityDistances.AddLast(info);
                }
                else if (info.Distance >= cityDistances.Last.Value.Distance)
                {
                    // More distant than most distant seen so far.
                    // Append to end, if we have capacity
                    if (cityDistances.Count < k)
                        cityDistances.AddLast(info);
                }
                else
                {
                    // Must be at least 1 element in the list, and we know our distance is less than the last one
                    // Find first node whose distance is greater than ours and insert ourselves before it          
                    for (LinkedListNode<CityDistance> node = cityDistances.First; node != null; node = node.Next)
                    {
                        if (info.Distance < node.Value.Distance)
                        {
                            cityDistances.AddBefore(node, info);
                            break;
                        }
                    }

                    // If we are now over capacity remove last (most distant) entry
                    if (cityDistances.Count > k)
                        cityDistances.RemoveLast();
                }
            }

            return new List<CityDistance>(cityDistances);
        }

        /// <summary>
        /// Helper: asserts a header has the expected content
        /// </summary>
        /// <param name="headers">Array of headers</param>
        /// <param name="colId">0-based column index</param>
        /// <param name="expectedValue">Expected value</param>
        static void AssertHeader(string[] headers, int colId, string expectedValue)
        {
            string actualValue = headers[colId];
            if (actualValue != expectedValue)
                Debug.Assert(false,
                    String.Format("worldcities.csv header {0}: expected '{1}', got '{2}'!", colId, expectedValue, actualValue)
                );
        }

        /// <summary>
        /// Creates a random temp folder and returns the path.
        /// </summary>
        /// <returns>Temp folder path.</returns>
        static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
