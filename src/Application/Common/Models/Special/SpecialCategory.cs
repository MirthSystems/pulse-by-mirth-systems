namespace Application.Common.Models.Special;

/// <summary>
/// Model for special category
/// </summary>
public class SpecialCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
}
