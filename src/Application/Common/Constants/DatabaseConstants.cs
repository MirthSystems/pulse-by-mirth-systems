namespace Application.Common.Constants;

/// <summary>
/// Database-related constants for entity configurations and constraints
/// </summary>
public static class DatabaseConstants
{
    // Table names
    public static class TableNames
    {
        public const string Users = "users";
        public const string Venues = "venues";
        public const string VenueCategories = "venue_categories";
        public const string Specials = "specials";
        public const string SpecialCategories = "special_categories";
        public const string BusinessHours = "business_hours";
        public const string DaysOfWeek = "days_of_week";
        public const string UserVenuePermissions = "user_venue_permissions";
        public const string VenueInvitations = "venue_invitations";
    }
    
    // Column lengths (matching your validation constants)
    public static class ColumnLengths
    {
        public const int Name = 100;
        public const int Description = 1000;
        public const int Address = 200;
        public const int Phone = 20;
        public const int Email = 255;
        public const int Url = 500;
        public const int ShortText = 50;
        public const int MediumText = 250;
        public const int LongText = 2000;
        public const int Code = 10;
        public const string SubIdentifier = "255"; // Auth0 sub can be long
        public const int PermissionType = 100;
        public const int Notes = 500;
    }
    
    // Index names
    public static class IndexNames
    {
        public const string UsersSub = "IX_Users_Sub";
        public const string UsersEmail = "IX_Users_Email";
        public const string VenuesLocation = "IX_Venues_Location";
        public const string VenuesCategory = "IX_Venues_CategoryId";
        public const string SpecialsVenue = "IX_Specials_VenueId";
        public const string SpecialsCategory = "IX_Specials_CategoryId";
        public const string SpecialsIsActive = "IX_Specials_IsActive";
        public const string BusinessHoursVenue = "IX_BusinessHours_VenueId";
        public const string PermissionsUserVenue = "IX_UserVenuePermissions_UserId_VenueId";
        public const string InvitationsEmail = "IX_VenueInvitations_Email";
        public const string InvitationsVenue = "IX_VenueInvitations_VenueId";
    }
    
    // Constraint names
    public static class ConstraintNames
    {
        public const string UserSubUnique = "UQ_Users_Sub";
        public const string UserEmailUnique = "UQ_Users_Email";
        public const string VenueNameUnique = "UQ_Venues_Name";
        public const string CategoryNameUnique = "UQ_Categories_Name";
        public const string UserVenuePermissionUnique = "UQ_UserVenuePermissions_UserId_VenueId";
    }
    
    // Connection string keys
    public static class ConnectionStringKeys
    {
        public const string DefaultConnection = "DefaultConnection";
        public const string PostgreSqlConnection = "PostgreSqlConnection";
        public const string RedisConnection = "RedisConnection";
    }
}
