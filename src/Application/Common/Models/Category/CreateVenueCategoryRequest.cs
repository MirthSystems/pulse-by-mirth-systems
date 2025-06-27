namespace Application.Common.Models.Category;

public record CreateVenueCategoryRequest(
    string Name,
    string? Description,
    string? Icon,
    bool IsActive = true
);
