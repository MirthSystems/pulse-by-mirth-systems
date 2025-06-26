namespace Application.Common.Models.Category;

public record UpdateVenueCategoryRequest(
    string Name,
    string? Description,
    string? Icon,
    bool IsActive
);
