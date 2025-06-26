namespace Application.Common.Models.Category;

public record UpdateSpecialCategoryRequest(
    string Name,
    string? Description,
    string? Icon,
    bool IsActive
);
