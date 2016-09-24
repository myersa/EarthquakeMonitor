using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EarthquakeMonitor.Properties;

namespace EarthquakeMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GeoJSON.PollingClient pollingClient;
        WorldCities.WorldCities cities;

        /// <summary>
        /// Databinding object: displays info about a single eartquake
        /// </summary>
        class EarthquakeRecord
        {
            public EarthquakeRecord(DateTime timestamp, double? magnitude, double longitude, double latitude, double depth, string nearestCities)
            {
                Timestamp = timestamp;
                Magnitude = magnitude;
                Longitude = longitude;
                Latitude = latitude;
                Depth = depth;
                NearestCities = nearestCities;
            }

            public DateTime Timestamp { get; private set; }
            public double? Magnitude { get; private set; }
            public double Longitude { get; private set; }
            public double Latitude { get; private set; }
            public double Depth { get; private set; }
            public string NearestCities { get; private set; }
        }

        ObservableCollection<EarthquakeRecord> earthquakes = new ObservableCollection<EarthquakeRecord>();

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            dataGrid.DataContext = earthquakes;

            // Add custom trace listeners that will update the GUI
            Trace.Listeners.Add(new EarthquakeMonitorTraceListener());
        }

        /// <summary>
        /// Handles the <see cref="Window.Loaded"/> event. Performs initialization.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Trace.TraceInformation("Loading cities data ...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            cities = new WorldCities.WorldCities();
            cities.Load();
            sw.Stop();
            Trace.TraceInformation("Loading cities took {0:.##} s.", sw.ElapsedMilliseconds / 1000.0);

            Trace.TraceInformation("Starting.");
            pollingClient = new GeoJSON.PollingClient(new GeoJSON.Client(), OnDataReceived, Settings.Default.Earthquakes_PollingIntervalMs);
        }

        /// <summary>
        /// Called when the <see cref="GeoJSON.PollingClient"/> gets new data. Need not be called on the UI thread.
        /// </summary>
        /// <param name="data">Retrieved data.</param>
        void OnDataReceived(GeoJSON.RootObject data)
        {
            Dispatcher.BeginInvoke(new Action(() => DisplayEarthquakes(data)));
        }

        /// <summary>
        /// Updates the UI after new earthquake data is retrieved. *Must* be called on the UI thread.
        /// </summary>
        /// <param name="data"></param>
        void DisplayEarthquakes(GeoJSON.RootObject data)
        {
            earthquakes.Clear();

            foreach (var feature in data.features)
            {
                GeoJSON.Properties props = feature.properties;
                List<double> coords = feature.geometry.coordinates;
                double longitude = coords[1];
                double latitude = coords[0];
                double depth = coords[2];

                List<WorldCities.CityDistance> nearestCities = cities.GetNearestCities(new GeoCoordinate(longitude, latitude));

                var rec = new EarthquakeRecord(
                    GeoJSON.Utils.TimeStampToDateTime(props.time),
                    props.mag,
                    longitude,
                    latitude,
                    depth,
                    string.Join(", ", nearestCities.ConvertAll(new Converter<WorldCities.CityDistance, string>((c) => c.ToString())))
                );

                earthquakes.Add(rec);
            }
        }
    }
}
