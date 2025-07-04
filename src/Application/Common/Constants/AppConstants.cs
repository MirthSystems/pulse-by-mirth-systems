namespace Application.Common.Constants;

/// <summary>
/// Application-wide configuration constants
/// </summary>
public static class AppConstants
{
    // Pagination defaults
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
    public const int MinPageSize = 1;
    
    // Search defaults
    public const int DefaultSearchRadius = 5000; // meters
    public const int MaxSearchRadius = 50000; // meters
    public const int MinSearchRadius = 100; // meters
    
    // File upload limits
    public const int MaxFileSize = 5 * 1024 * 1024; // 5MB
    
    // Allowed image types (readonly since arrays cannot be const)
    public static readonly string[] AllowedImageTypes = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    
    // Cache durations (in minutes)
    public const int ShortCacheDuration = 5;
    public const int MediumCacheDuration = 30;
    public const int LongCacheDuration = 120;
    
    // Rate limiting
    public const int DefaultRateLimit = 100; // requests per minute
    public const int AuthenticatedRateLimit = 1000; // requests per minute
    
    // Date/Time formats
    public const string DateFormat = "yyyy-MM-dd";
    public const string TimeFormat = "HH:mm";
    public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    public const string IsoDateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
    
    // Business rules
    public const int InvitationExpiryDays = 7;
    public const int MaxBusinessHoursPerDay = 3; // for split shifts
    public const int MaxSpecialsPerVenue = 50;
    public const int MaxVenuesPerUser = 10;
    
    // Validation rules
    public const int MinPasswordLength = 8;
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 1000;
    public const int MaxAddressLength = 200;
    public const int MaxPhoneLength = 20;
    public const int MaxEmailLength = 255;
    public const int MaxUrlLength = 500;
    
    // Feature flags
    public const string FeatureAzureMaps = "AzureMaps";
    public const string FeatureGeospatialSearch = "GeospatialSearch";
    public const string FeatureFileUpload = "FileUpload";
    public const string FeatureEmailNotifications = "EmailNotifications";
    public const string FeatureWebhooks = "Webhooks";
}
