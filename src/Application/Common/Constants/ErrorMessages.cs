namespace Application.Common.Constants;

/// <summary>
/// Standard error messages for consistent error handling
/// </summary>
public static class ErrorMessages
{
    // General errors
    public const string InternalServerError = "An internal server error occurred.";
    public const string ValidationError = "Validation failed.";
    public const string NotFound = "The requested resource was not found.";
    public const string Unauthorized = "You are not authorized to access this resource.";
    public const string Forbidden = "You do not have permission to perform this action.";
    public const string BadRequest = "The request was invalid.";
    public const string Conflict = "The request conflicts with the current state of the resource.";
    
    // Authentication errors
    public const string InvalidCredentials = "Invalid credentials provided.";
    public const string TokenExpired = "Your session has expired. Please log in again.";
    public const string UserNotFound = "User not found.";
    public const string UserInactive = "User account is inactive.";
    
    // Venue errors
    public const string VenueNotFound = "Venue not found.";
    public const string VenueInactive = "Venue is inactive.";
    public const string VenueNameRequired = "Venue name is required.";
    public const string VenueAddressRequired = "Venue address is required.";
    public const string VenueCategoryRequired = "Venue category is required.";
    public const string VenueAlreadyExists = "A venue with this name already exists.";
    public const string VenueCannotBeDeleted = "Venue cannot be deleted because it has active specials.";
    
    // Special errors
    public const string SpecialNotFound = "Special not found.";
    public const string SpecialInactive = "Special is inactive.";
    public const string SpecialTitleRequired = "Special title is required.";
    public const string SpecialDescriptionRequired = "Special description is required.";
    public const string SpecialDateRequired = "Special date is required.";
    public const string SpecialTimeRequired = "Special time is required.";
    public const string SpecialEndDateBeforeStart = "Special end date cannot be before start date.";
    public const string SpecialCronScheduleInvalid = "Special cron schedule is invalid.";
    
    // Permission errors
    public const string PermissionNotFound = "Permission not found.";
    public const string PermissionAlreadyExists = "Permission already exists for this user and venue.";
    public const string PermissionCannotBeRevoked = "Permission cannot be revoked.";
    public const string InsufficientPermissions = "You do not have sufficient permissions to perform this action.";
    public const string CannotRevokeOwnPermission = "You cannot revoke your own permission.";
    public const string CannotRevokeHigherPermission = "You cannot revoke a permission higher than your own.";
    
    // Invitation errors
    public const string InvitationNotFound = "Invitation not found.";
    public const string InvitationExpired = "Invitation has expired.";
    public const string InvitationAlreadyAccepted = "Invitation has already been accepted.";
    public const string InvitationAlreadyDeclined = "Invitation has already been declined.";
    public const string InvitationCannotBeCanceled = "Invitation cannot be canceled.";
    public const string InvitationEmailRequired = "Invitation email is required.";
    public const string InvitationVenueRequired = "Invitation venue is required.";
    public const string InvitationPermissionRequired = "Invitation permission is required.";
    public const string UserAlreadyHasAccess = "User already has access to this venue.";
    
    // Location errors
    public const string LocationNotFound = "Location not found.";
    public const string GeocodeServiceUnavailable = "Geocoding service is unavailable.";
    public const string InvalidCoordinates = "Invalid coordinates provided.";
    public const string AddressRequired = "Address is required.";
    
    // Search errors
    public const string SearchTermRequired = "Search term is required.";
    public const string SearchParametersInvalid = "Search parameters are invalid.";
    public const string SearchRadiusInvalid = "Search radius must be between 1 and 50000 meters.";
    public const string SearchPageSizeInvalid = "Page size must be between 1 and 100.";
    public const string SearchPageNumberInvalid = "Page number must be greater than 0.";
    
    // File upload errors
    public const string FileRequired = "File is required.";
    public const string FileTypeNotSupported = "File type is not supported.";
    public const string FileSizeExceeded = "File size exceeds maximum limit.";
    public const string FileUploadFailed = "File upload failed.";
    
    // Rate limiting errors
    public const string RateLimitExceeded = "Rate limit exceeded. Please try again later.";
    public const string TooManyRequests = "Too many requests. Please slow down.";
}
