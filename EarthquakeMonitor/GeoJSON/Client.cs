using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using EarthquakeMonitor.Properties;

namespace EarthquakeMonitor.GeoJSON
{
    /// <summary>
    /// Async REST client for USGS' earthquakes resource.
    /// </summary>
    class Client : IDisposable
    {
        private HttpClient httpClient;

        /// <summary>
        /// Constructor
        /// </summary>
        public Client()
        {
            // Initialize underlying HTTP client
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Settings.Default.Earthquakes_BaseUri);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <inheritDoc/>
        public void Dispose()
        {
            httpClient.Dispose();
        }

        /// <summary>
        /// Perform a GET request against the earthquakes API and returns the retrieved data.
        /// </summary>
        /// <param name="resource">Resource to get, relative to <see cref="Settings.Default.EarthquakesBaseUri"/>. 
        /// Default is <see cref="Settings.Default.EarthquakesResAllHour"/>.</param>
        /// <returns>Async task returning a <see cref="RootObject"/>.</returns>
        public async Task<RootObject> GetAsAsync(string resource = null)
        {
            // apply default values
            if (resource == null)
                resource = Settings.Default.Earthquakes_ResAllHour;

            HttpResponseMessage response = await httpClient.GetAsync(resource);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<RootObject>();
        }
    }
}
