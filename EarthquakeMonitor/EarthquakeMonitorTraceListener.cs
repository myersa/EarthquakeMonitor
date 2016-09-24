using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EarthquakeMonitor
{
    /// <summary>
    /// Trace listener for this application: writes messages to the main window's "logView" textbox.
    /// </summary>
    class EarthquakeMonitorTraceListener : TraceListener
    {
        /// <inheritdoc/>
        public override void Write(string message)
        {
            Action updateUi = () => {
                App.Current.MainWindow.logView.AppendText(DateTime.Now.ToLongTimeString());
                App.Current.MainWindow.logView.AppendText(": ");
                App.Current.MainWindow.logView.AppendText(message);
                App.Current.MainWindow.logView.AppendText("\n");
            };

            if (App.Current.Dispatcher.CheckAccess())
            {
                updateUi();
            }
            else
            {
                App.Current.Dispatcher.BeginInvoke(updateUi);
            }
        }

        /// <inheritdoc/>
        public override void WriteLine(string message)
        {
            Write(message);
        }

        /// <inheritdoc/>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            TraceEvent(
                eventCache,
                source,
                eventType,
                id,
                string.Format(format, args)
            );
        }

        /// <inheritdoc/>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            Write(message);
        }
    }
}
