using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EarthquakeMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Gets the current instance of the application object.
        /// </summary>
        public static new App Current { get { return (App)Application.Current; } }

        /// <summary>
        /// Gets a type-specific reference to the main window.
        /// </summary>
        public new MainWindow MainWindow
        {
            get { return (MainWindow)base.MainWindow; }
            set { base.MainWindow = value; }
        }
    }
}
