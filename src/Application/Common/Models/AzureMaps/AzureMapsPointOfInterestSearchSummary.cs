namespace Application.Common.Models.AzureMaps;

internal class AzureMapsPointOfInterestSearchSummary
{
    public string? Query { get; set; }
    public int NumResults { get; set; }
    public int TotalResults { get; set; }
    public int Offset { get; set; }
}
