namespace Application.Common.Models.Category;

public record VenueCategoryResponse(
    int Id,
    string Name,
    string? Description,
    string? Icon,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
