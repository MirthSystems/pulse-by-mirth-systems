namespace Application.Common.Models.Category;

public record CreateSpecialCategoryRequest(
    string Name,
    string? Description,
    string? Icon,
    bool IsActive = true
);
