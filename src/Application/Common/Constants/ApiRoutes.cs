namespace Application.Common.Constants;

/// <summary>
/// API route constants for consistent routing across controllers
/// </summary>
public static class ApiRoutes
{
    public const string Base = "api";
    
    // Venue routes
    public static class Venues
    {
        public const string Base = $"{ApiRoutes.Base}/venues";
        public const string GetAll = Base;
        public const string GetById = $"{Base}/{{id:long}}";
        public const string GetActive = $"{Base}/active";
        public const string GetByCategory = $"{Base}/category/{{categoryId:int}}";
        public const string GetWithSpecials = $"{Base}/with-specials";
        public const string GetNear = $"{Base}/near";
        public const string Search = $"{Base}/search";
        public const string SearchEnhanced = $"{Base}/search/enhanced";
        public const string Categories = $"{Base}/categories";
        public const string CategoryById = $"{Base}/categories/{{id:int}}";
        
        // Management routes (require authentication)
        public const string Create = Base;
        public const string Update = $"{Base}/{{id:long}}";
        public const string Delete = $"{Base}/{{id:long}}";
        public const string MyVenues = $"{Base}/my";
        
        // Azure Maps integration routes  
        public const string Geocode = $"{Base}/{{id:long}}/geocode";
        public const string LocationDetails = $"{Base}/{{id:long}}/location-details";
        public const string Timezone = $"{Base}/{{id:long}}/timezone";
        public const string NearbyPOIs = $"{Base}/{{id:long}}/nearby-pois";
    }
    
    // Special routes
    public static class Specials
    {
        public const string Base = $"{ApiRoutes.Base}/specials";
        public const string GetAll = Base;
        public const string GetById = $"{Base}/{{id:long}}";
        public const string GetActive = $"{Base}/active";
        public const string GetByCategory = $"{Base}/category/{{categoryId:int}}";
        public const string GetActiveByCategoryFull = $"{Base}/category/{{categoryId:int}}/active";
        public const string GetByVenue = $"{Base}/venue/{{venueId:long}}";
        public const string GetActiveByVenue = $"{Base}/venue/{{venueId:long}}/active";
        public const string GetRecurring = $"{Base}/recurring";
        public const string GetNow = $"{Base}/now";
        public const string GetNear = $"{Base}/near";
        public const string Search = $"{Base}/search";
        public const string SearchVenues = $"{Base}/search/venues";
        public const string Categories = $"{Base}/categories";
        public const string CategoryById = $"{Base}/categories/{{id:int}}";
        
        // Management routes (require authentication)
        public const string Create = $"{Venues.Base}/{{venueId:long}}/specials";
        public const string Update = $"{Base}/{{id:long}}";
        public const string Delete = $"{Base}/{{id:long}}";
        public const string MySpecials = $"{Base}/my";
    }
    
    // Permission routes
    public static class Permissions
    {
        public const string Base = $"{ApiRoutes.Base}/permissions";
        public const string GetMine = $"{Base}/my";
        public const string GetByUser = $"{Base}/users/{{userId:long}}";
        public const string GetByVenue = $"{Venues.Base}/{{venueId:long}}/permissions";
        public const string Update = $"{Base}/{{permissionId:long}}";
        public const string Revoke = $"{Base}/{{permissionId:long}}";
        public const string Types = $"{Base}/types";
    }
    
    // Invitation routes
    public static class Invitations
    {
        public const string Base = $"{ApiRoutes.Base}/invitations";
        public const string Create = Base;
        public const string GetMine = $"{Base}/my";
        public const string GetByVenue = $"{Venues.Base}/{{venueId:long}}/invitations";
        public const string Accept = $"{Base}/{{invitationId:long}}/accept";
        public const string Decline = $"{Base}/{{invitationId:long}}/decline";
        public const string Cancel = $"{Base}/{{invitationId:long}}";
    }
    
    // Location routes
    public static class Location
    {
        public const string Base = $"{ApiRoutes.Base}/location";
        public const string Search = $"{Base}/search";
        public const string Geocode = $"{Base}/geocode";
        public const string ReverseGeocode = $"{Base}/reverse-geocode";
        public const string Timezone = $"{Base}/timezone";
        public const string ValidateAddress = $"{Base}/validate-address";
    }
    
    // User routes
    public static class Users
    {
        public const string Base = $"{ApiRoutes.Base}/users";
        public const string Sync = $"{Base}/sync";
        public const string Profile = $"{Base}/profile";
    }
}
