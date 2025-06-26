namespace Application.Common.Models.AzureMaps;

internal class AzureMapsPointOfInterestSearchResult
{
    public string? Id { get; set; }
    public double Score { get; set; }
    public double Dist { get; set; }
    public AzureMapsPointOfInterest? Poi { get; set; }
    public AzureMapsAddress? Address { get; set; }
    public AzureMapsPosition? Position { get; set; }
}
