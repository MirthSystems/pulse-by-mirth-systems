using Azure.Maps.Geolocation;
using Azure.Maps.Search;
using Azure.Maps.TimeZones;

namespace Application.Common.Interfaces;

public interface IAzureMapsService
{
    MapsSearchClient SearchClient { get; }
    MapsGeolocationClient GeolocationClient { get; }
    MapsTimeZoneClient TimeZoneClient { get; }
}
