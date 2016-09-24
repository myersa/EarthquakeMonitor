# EarthquakeMonitor

Welcome to EarthquakeMonitor!

This is a .NET WPF Desktop application written in C#.

It gets live earthquake info from [GeoJSON](http://earthquake.usgs.gov/earthquakes/feed/v1.0/geojson.php)
and displays all earthquakes that occurred in the last hour, including the nearest 3 cities.

## Development Environment

* Microsoft Visual Studio Community 2015 Version 14.0.25431.01 Update 3
* Microsoft .NET Framework version 4.6.01586

## Building the Solution

1. `git clone https://github.com/myersa/EarthquakeMonitor.git`
1. Open `/EarthquakeMonitor.sln` in Visual Studio 2015 or newer
1. Install dependencies: 
    * Open the NuGet console (Tools -> NuGet Package Manager)
    * Install Microsoft.AspNet.WebApi.Client: `Install-Package Microsoft.AspNet.WebApi.Client`
1. Build
1. Run
