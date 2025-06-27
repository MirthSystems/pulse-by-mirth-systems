namespace Application.Common.Models.AzureMaps;

internal class AzureMapsPointOfInterest
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Url { get; set; }
    public List<AzureMapsClassification>? Classifications { get; set; }
}
