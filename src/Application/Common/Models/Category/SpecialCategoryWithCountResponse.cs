namespace Application.Common.Models.Category;

public record SpecialCategoryWithCountResponse(
    int Id,
    string Name,
    string? Description,
    string? Icon,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int SpecialCount
);
