using Application.Common.Interfaces;

using Azure;
using Azure.Maps.Geolocation;
using Azure.Maps.Search;
using Azure.Maps.TimeZones;

namespace Application.Infrastructure.Services;

public class AzureMapsService : IAzureMapsService
{
    public MapsSearchClient SearchClient { get; }
    public MapsGeolocationClient GeolocationClient { get; }    
    public MapsTimeZoneClient TimeZoneClient { get; }

    public AzureMapsService(AzureKeyCredential azureMapsKeyCredential)
    {
        SearchClient = new MapsSearchClient(azureMapsKeyCredential);
        GeolocationClient = new MapsGeolocationClient(azureMapsKeyCredential);       
        TimeZoneClient = new MapsTimeZoneClient(azureMapsKeyCredential);
    }
}
