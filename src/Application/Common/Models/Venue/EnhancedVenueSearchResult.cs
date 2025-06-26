using Application.Common.Models.Location;
using Application.Common.Models.Search;

namespace Application.Common.Models.Venue;

/// <summary>
/// Enhanced venue search result that combines database results with Azure Maps POIs
/// </summary>
public class EnhancedVenueSearchResult
{
    public PagedResponse<VenueSummary> DatabaseResults { get; set; } = new();
    public List<PointOfInterest> AzureMapsPOIs { get; set; } = new();
    public int TotalDatabaseResults => DatabaseResults.TotalCount;
    public int TotalAzureMapsPOIs => AzureMapsPOIs.Count;
}
