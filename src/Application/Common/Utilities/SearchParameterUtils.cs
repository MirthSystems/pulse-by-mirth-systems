using Application.Common.Constants;
using Application.Common.Models.Search;

namespace Application.Common.Utilities;

/// <summary>
/// Utility for validating and normalizing search parameters
/// Ensures consistent validation across all search endpoints
/// </summary>
public static class SearchParameterUtils
{
    /// <summary>
    /// Validates venue search parameters
    /// </summary>
    public static (bool isValid, List<string> errors) ValidateVenueSearch(VenueSearch search)
    {
        var errors = new List<string>();
        
        // Validate page parameters
        if (search.PageNumber <= 0)
        {
            errors.Add("Page number must be greater than 0");
        }
        
        if (search.PageSize <= 0 || search.PageSize > AppConstants.MaxPageSize)
        {
            errors.Add($"Page size must be between 1 and {AppConstants.MaxPageSize}");
        }
        
        // Validate search radius if coordinates provided
        if (search.Latitude.HasValue && search.Longitude.HasValue)
        {
            if (!search.RadiusInMeters.HasValue || 
                search.RadiusInMeters < AppConstants.MinSearchRadius || 
                search.RadiusInMeters > AppConstants.MaxSearchRadius)
            {
                errors.Add($"Search radius must be between {AppConstants.MinSearchRadius} and {AppConstants.MaxSearchRadius} meters");
            }
            
            if (!IsValidLatitude(search.Latitude.Value))
            {
                errors.Add("Latitude must be between -90 and 90 degrees");
            }
            
            if (!IsValidLongitude(search.Longitude.Value))
            {
                errors.Add("Longitude must be between -180 and 180 degrees");
            }
        }
        
        // Validate search term length if provided
        if (!string.IsNullOrEmpty(search.SearchTerm) && search.SearchTerm.Length > 100)
        {
            errors.Add("Search term cannot exceed 100 characters");
        }
        
        // Validate sort parameters
        if (!string.IsNullOrEmpty(search.SortBy) && 
            !new[] { "name", "distance", "category" }.Contains(search.SortBy.ToLower()))
        {
            errors.Add("Sort by must be one of: name, distance, category");
        }
        
        if (!string.IsNullOrEmpty(search.SortOrder) && 
            !new[] { "asc", "desc" }.Contains(search.SortOrder.ToLower()))
        {
            errors.Add("Sort order must be either 'asc' or 'desc'");
        }
        
        return (errors.Count == 0, errors);
    }
    
    /// <summary>
    /// Validates special search parameters
    /// </summary>
    public static (bool isValid, List<string> errors) ValidateSpecialSearch(SpecialSearch search)
    {
        var errors = new List<string>();
        
        // Validate page parameters
        if (search.PageNumber <= 0)
        {
            errors.Add("Page number must be greater than 0");
        }
        
        if (search.PageSize <= 0 || search.PageSize > AppConstants.MaxPageSize)
        {
            errors.Add($"Page size must be between 1 and {AppConstants.MaxPageSize}");
        }
        
        // Validate date range if provided
        if (!string.IsNullOrEmpty(search.StartDate) && !string.IsNullOrEmpty(search.EndDate))
        {
            if (DateTime.TryParse(search.StartDate, out var startDate) && 
                DateTime.TryParse(search.EndDate, out var endDate))
            {
                if (startDate > endDate)
                {
                    errors.Add("Start date cannot be after end date");
                }
            }
            else
            {
                errors.Add("Invalid date format. Use yyyy-MM-dd");
            }
        }
        
        // Validate search radius if coordinates provided
        if (search.Latitude.HasValue && search.Longitude.HasValue)
        {
            if (!search.RadiusInMeters.HasValue || 
                search.RadiusInMeters < AppConstants.MinSearchRadius || 
                search.RadiusInMeters > AppConstants.MaxSearchRadius)
            {
                errors.Add($"Search radius must be between {AppConstants.MinSearchRadius} and {AppConstants.MaxSearchRadius} meters");
            }
            
            if (!IsValidLatitude(search.Latitude.Value))
            {
                errors.Add("Latitude must be between -90 and 90 degrees");
            }
            
            if (!IsValidLongitude(search.Longitude.Value))
            {
                errors.Add("Longitude must be between -180 and 180 degrees");
            }
        }
        
        // Validate sort parameters
        if (!string.IsNullOrEmpty(search.SortBy) && 
            !new[] { "venue", "distance", "category", "date" }.Contains(search.SortBy.ToLower()))
        {
            errors.Add("Sort by must be one of: venue, distance, category, date");
        }
        
        if (!string.IsNullOrEmpty(search.SortOrder) && 
            !new[] { "asc", "desc" }.Contains(search.SortOrder.ToLower()))
        {
            errors.Add("Sort order must be either 'asc' or 'desc'");
        }
        
        return (errors.Count == 0, errors);
    }
    
    /// <summary>
    /// Normalizes venue search parameters with defaults
    /// </summary>
    public static void NormalizeVenueSearch(VenueSearch search)
    {
        search.PageNumber = Math.Max(1, search.PageNumber);
        search.PageSize = Math.Clamp(search.PageSize, AppConstants.MinPageSize, AppConstants.MaxPageSize);
        
        if (search.RadiusInMeters.HasValue)
        {
            search.RadiusInMeters = Math.Clamp(search.RadiusInMeters.Value, AppConstants.MinSearchRadius, AppConstants.MaxSearchRadius);
        }
        
        search.SearchTerm = search.SearchTerm?.Trim();
        search.SortBy = search.SortBy?.ToLower() ?? "name";
        search.SortOrder = search.SortOrder?.ToLower() ?? "asc";
    }
    
    /// <summary>
    /// Normalizes special search parameters with defaults
    /// </summary>
    public static void NormalizeSpecialSearch(SpecialSearch search)
    {
        search.PageNumber = Math.Max(1, search.PageNumber);
        search.PageSize = Math.Clamp(search.PageSize, AppConstants.MinPageSize, AppConstants.MaxPageSize);
        
        if (search.RadiusInMeters.HasValue)
        {
            search.RadiusInMeters = Math.Clamp(search.RadiusInMeters.Value, AppConstants.MinSearchRadius, AppConstants.MaxSearchRadius);
        }
        
        search.SearchTerm = search.SearchTerm?.Trim();
        search.SortBy = search.SortBy?.ToLower() ?? "venue";
        search.SortOrder = search.SortOrder?.ToLower() ?? "asc";
    }
    
    /// <summary>
    /// Validates latitude coordinate
    /// </summary>
    private static bool IsValidLatitude(double latitude)
    {
        return latitude >= -90.0 && latitude <= 90.0;
    }
    
    /// <summary>
    /// Validates longitude coordinate
    /// </summary>
    private static bool IsValidLongitude(double longitude)
    {
        return longitude >= -180.0 && longitude <= 180.0;
    }
}
