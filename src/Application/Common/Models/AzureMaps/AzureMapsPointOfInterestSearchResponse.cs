namespace Application.Common.Models.AzureMaps;

internal class AzureMapsPointOfInterestSearchResponse
{
    public AzureMapsPointOfInterestSearchSummary? Summary { get; set; }
    public List<AzureMapsPointOfInterestSearchResult>? Results { get; set; }
}
