﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EarthquakeMonitor.Properties;

namespace EarthquakeMonitor.GeoJSON
{
    /// <summary>
    /// Signature for callback to be invoked on data retrieved.
    /// </summary>
    /// <param name="data">Data retrieved.</param>
    delegate void DataReceivedCallback(RootObject data);

    /// <summary>
    /// Wraps a <see cref="Client"/>. Calls its <see cref="Client.GetAsAsync"/> method periodically.
    /// </summary>
    class PollingClient
    {
        Client geoJsonClient;
        DataReceivedCallback callback;
        Timer clientTimer;
        Task getDataTask = null;
        long? lastGenerated = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">Asyncronous REST client instance</param>
        /// <param name="callback">Callback to be invoked on data retrieved</param>
        /// <param name="intervalMs">Polling interval, in milliseconds.</param>
        public PollingClient(Client client, DataReceivedCallback callback, int intervalMs)
        {
            geoJsonClient = client;
            this.callback = callback; 
            clientTimer = new Timer(onTimerTick, null, 0, intervalMs);
        }

        void onTimerTick(object state)
        {
            if (getDataTask != null && !getDataTask.IsCompleted)
            {
                Trace.TraceInformation("GeoJSON client: previous get-data task still running - doing nothing.");
                return;
            }
            
            getDataTask = GetData();
        }

        async Task GetData()
        {
            RootObject data = await geoJsonClient.GetAsAsync();

            // Don't call if metadata.generated is the same as last time
            if (lastGenerated == null || data.metadata.generated > lastGenerated)
            {
                Trace.TraceInformation(
                    "GeoJSON client: got {0} earthquakes (data generated by server at {1})", 
                    data.features.Count,
                    Utils.TimeStampToDateTime(data.metadata.generated)
                );

                callback(data);
                lastGenerated = data.metadata.generated;
            }
            else
            {
                Trace.TraceInformation("GeoJSON client: no new data available.");
            }
        }
    }
}
