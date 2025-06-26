namespace Application.Common.Models.Category;

public record SpecialCategoryResponse(
    int Id,
    string Name,
    string? Description,
    string? Icon,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
