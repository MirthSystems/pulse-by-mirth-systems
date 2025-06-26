using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Models.Location;
using Application.Common.Models.Search;
using Application.Common.Models.Venue;
using Application.Domain.Entities;
using NetTopologySuite.Geometries;
using NodaTime;
using NodaTime.Text;

namespace Application.Infrastructure.Services;

/// <summary>
/// Service implementation for venue operations
/// </summary>
public class VenueService : IVenueService
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueCategoryRepository _venueCategoryRepository;
    private readonly IAzureMapsService _azureMapsService;
    private readonly IClock _clock;

    public VenueService(
        IVenueRepository venueRepository,
        IVenueCategoryRepository venueCategoryRepository,
        IAzureMapsService azureMapsService,
        IClock clock)
    {
        _venueRepository = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
        _venueCategoryRepository = venueCategoryRepository ?? throw new ArgumentNullException(nameof(venueCategoryRepository));
        _azureMapsService = azureMapsService ?? throw new ArgumentNullException(nameof(azureMapsService));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task<ApiResponse<Venue?>> GetVenueByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var venue = await _venueRepository.GetVenueWithDetailsAsync(id, cancellationToken);
            if (venue == null)
            {
                return ApiResponse<Venue?>.ErrorResult($"Venue with ID {id} not found.");
            }

            var venueModel = MapToVenue(venue);
            return ApiResponse<Venue?>.SuccessResult(venueModel);
        }
        catch (Exception ex)
        {
            return ApiResponse<Venue?>.ErrorResult($"Error retrieving venue: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<VenueSummary>>> GetAllVenuesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var venues = await _venueRepository.GetAllAsync(cancellationToken);
            var Venues = venues.Select(v => MapToVenueSummary(v));
            return ApiResponse<IEnumerable<VenueSummary>>.SuccessResult(Venues);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<VenueSummary>>.ErrorResult($"Error retrieving venues: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Venue>> CreateVenueAsync(CreateVenue createVenue, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify category exists
            var category = await _venueCategoryRepository.GetByIdAsync(createVenue.CategoryId, cancellationToken);
            if (category == null)
            {
                return ApiResponse<Venue>.ErrorResult($"Venue category with ID {createVenue.CategoryId} not found.");
            }

            var venue = MapToVenueEntity(createVenue);
            var createdVenue = await _venueRepository.AddAsync(venue, cancellationToken);
            await _venueRepository.SaveChangesAsync(cancellationToken);

            var venueWithDetails = await _venueRepository.GetVenueWithDetailsAsync(createdVenue.Id, cancellationToken);
            var Venue = MapToVenue(venueWithDetails!);

            return ApiResponse<Venue>.SuccessResult(Venue, "Venue created successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<Venue>.ErrorResult($"Error creating venue: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Venue?>> UpdateVenueAsync(long id, UpdateVenue updateVenue, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingVenue = await _venueRepository.GetByIdAsync(id, cancellationToken);
            if (existingVenue == null)
            {
                return ApiResponse<Venue?>.ErrorResult($"Venue with ID {id} not found.");
            }

            // Verify category exists
            var category = await _venueCategoryRepository.GetByIdAsync(updateVenue.CategoryId, cancellationToken);
            if (category == null)
            {
                return ApiResponse<Venue?>.ErrorResult($"Venue category with ID {updateVenue.CategoryId} not found.");
            }

            UpdateVenueEntity(existingVenue, updateVenue);
            await _venueRepository.UpdateAsync(existingVenue, cancellationToken);
            await _venueRepository.SaveChangesAsync(cancellationToken);

            var venueWithDetails = await _venueRepository.GetVenueWithDetailsAsync(id, cancellationToken);
            var Venue = MapToVenue(venueWithDetails!);

            return ApiResponse<Venue?>.SuccessResult(Venue, "Venue updated successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<Venue?>.ErrorResult($"Error updating venue: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteVenueAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var venue = await _venueRepository.GetByIdAsync(id, cancellationToken);
            if (venue == null)
            {
                return ApiResponse<bool>.ErrorResult($"Venue with ID {id} not found.");
            }

            await _venueRepository.DeleteAsync(venue, cancellationToken);
            await _venueRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResult(true, "Venue deleted successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResult($"Error deleting venue: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<VenueSummary>>> GetActiveVenuesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var venues = await _venueRepository.GetActiveVenuesAsync(cancellationToken);
            var Venues = venues.Select(v => MapToVenueSummary(v));
            return ApiResponse<IEnumerable<VenueSummary>>.SuccessResult(Venues);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<VenueSummary>>.ErrorResult($"Error retrieving active venues: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<VenueSummary>>> GetVenuesByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var venues = await _venueRepository.GetVenuesByCategoryAsync(categoryId, cancellationToken);
            var Venues = venues.Select(v => MapToVenueSummary(v));
            return ApiResponse<IEnumerable<VenueSummary>>.SuccessResult(Venues);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<VenueSummary>>.ErrorResult($"Error retrieving venues by category: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<VenueSummary>>> GetVenuesWithActiveSpecialsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var venues = await _venueRepository.GetVenuesWithActiveSpecialsAsync(cancellationToken);
            var Venues = venues.Select(v => MapToVenueSummary(v));
            return ApiResponse<IEnumerable<VenueSummary>>.SuccessResult(Venues);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<VenueSummary>>.ErrorResult($"Error retrieving venues with active specials: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<VenueSummary>>> GetVenuesNearLocationAsync(
        double latitude, 
        double longitude, 
        double radiusInMeters = 5000, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var venues = await _venueRepository.GetVenuesInRadiusAsync(latitude, longitude, radiusInMeters, cancellationToken);
            var Venues = venues.Select(v => MapToVenueSummary(v, latitude, longitude));
            return ApiResponse<IEnumerable<VenueSummary>>.SuccessResult(Venues);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<VenueSummary>>.ErrorResult($"Error retrieving venues near location: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PagedResponse<VenueSummary>>> SearchVenuesAsync(
        VenueSearch search, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var venues = await _venueRepository.SearchVenuesAsync(
                search.SearchTerm,
                search.CategoryId,
                search.Latitude,
                search.Longitude,
                search.RadiusInMeters,
                search.ActiveOnly,
                cancellationToken);

            var venueList = venues.ToList();
            var totalCount = venueList.Count;

            // Apply pagination
            var pagedVenues = venueList
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize);

            var Venues = pagedVenues.Select(v => search.Latitude.HasValue && search.Longitude.HasValue 
                ? MapToVenueSummary(v, search.Latitude.Value, search.Longitude.Value)
                : MapToVenueSummary(v));

            var pagedResponse = new PagedResponse<VenueSummary>
            {
                Items = Venues,
                TotalCount = totalCount,
                PageNumber = search.PageNumber,
                PageSize = search.PageSize
            };

            return ApiResponse<PagedResponse<VenueSummary>>.SuccessResult(pagedResponse);
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResponse<VenueSummary>>.ErrorResult($"Error searching venues: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<VenueCategory>>> GetVenueCategoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _venueCategoryRepository.GetCategoriesOrderedBySortOrderAsync(cancellationToken);
            var categoryDtos = categories.Select(c => MapToVenueCategory(c));
            return ApiResponse<IEnumerable<VenueCategory>>.SuccessResult(categoryDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<VenueCategory>>.ErrorResult($"Error retrieving venue categories: {ex.Message}");
        }
    }

    public async Task<ApiResponse<VenueCategory?>> GetVenueCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _venueCategoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                return ApiResponse<VenueCategory?>.ErrorResult($"Venue category with ID {id} not found.");
            }

            var categoryDto = MapToVenueCategory(category);
            return ApiResponse<VenueCategory?>.SuccessResult(categoryDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<VenueCategory?>.ErrorResult($"Error retrieving venue category: {ex.Message}");
        }
    }

    #region Private Mapping Methods

    private static Venue MapToVenue(VenueEntity venue)
    {
        return new Venue
        {
            Id = venue.Id,
            CategoryId = venue.CategoryId,
            Name = venue.Name,
            Description = venue.Description,
            PhoneNumber = venue.PhoneNumber,
            Website = venue.Website,
            Email = venue.Email,
            ProfileImage = venue.ProfileImage,
            StreetAddress = venue.StreetAddress,
            SecondaryAddress = venue.SecondaryAddress,
            Locality = venue.Locality,
            Region = venue.Region,
            PostalCode = venue.PostalCode,
            Country = venue.Country,
            Latitude = venue.Location?.Y,
            Longitude = venue.Location?.X,
            IsActive = venue.IsActive,
            Category = venue.Category != null ? MapToVenueCategory(venue.Category) : null,
            BusinessHours = venue.BusinessHours?.Select(MapToBusinessHours).ToList() ?? new List<BusinessHours>(),
            ActiveSpecials = venue.Specials?.Where(s => s.IsActive).Select(MapToSpecialSummary).ToList() ?? new List<Application.Common.Models.Special.SpecialSummary>()
        };
    }

    private static VenueSummary MapToVenueSummary(VenueEntity venue, double? userLatitude = null, double? userLongitude = null)
    {
        double? distance = null;
        if (userLatitude.HasValue && userLongitude.HasValue && venue.Location != null)
        {
            var userLocation = new Point(userLongitude.Value, userLatitude.Value) { SRID = 4326 };
            distance = venue.Location.Distance(userLocation);
        }

        return new VenueSummary
        {
            Id = venue.Id,
            CategoryId = venue.CategoryId,
            Name = venue.Name,
            Description = venue.Description,
            PhoneNumber = venue.PhoneNumber,
            Website = venue.Website,
            Email = venue.Email,
            ProfileImage = venue.ProfileImage,
            StreetAddress = venue.StreetAddress,
            SecondaryAddress = venue.SecondaryAddress,
            Locality = venue.Locality,
            Region = venue.Region,
            PostalCode = venue.PostalCode,
            Country = venue.Country,
            Latitude = venue.Location?.Y,
            Longitude = venue.Location?.X,
            IsActive = venue.IsActive,
            CategoryName = venue.Category?.Name ?? string.Empty,
            CategoryIcon = venue.Category?.Icon,
            ActiveSpecialsCount = venue.Specials?.Count(s => s.IsActive) ?? 0,
            DistanceInMeters = distance
        };
    }

    private static VenueCategory MapToVenueCategory(VenueCategoryEntity category)
    {
        return new VenueCategory
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Icon = category.Icon,
            SortOrder = category.SortOrder
        };
    }

    private static BusinessHours MapToBusinessHours(BusinessHoursEntity businessHours)
    {
        return new BusinessHours
        {
            Id = businessHours.Id,
            VenueId = businessHours.VenueId,
            DayOfWeekId = businessHours.DayOfWeekId,
            DayOfWeekName = businessHours.DayOfWeek?.Name ?? string.Empty,
            DayOfWeekShortName = businessHours.DayOfWeek?.ShortName ?? string.Empty,
            OpenTime = businessHours.OpenTime?.ToString("HH:mm", null),
            CloseTime = businessHours.CloseTime?.ToString("HH:mm", null),
            IsClosed = businessHours.IsClosed,
            SortOrder = businessHours.DayOfWeek?.SortOrder ?? 0
        };
    }

    private static Application.Common.Models.Special.SpecialSummary MapToSpecialSummary(SpecialEntity special)
    {
        return new Application.Common.Models.Special.SpecialSummary
        {
            Id = special.Id,
            VenueId = special.VenueId,
            VenueName = special.Venue?.Name ?? string.Empty,
            SpecialCategoryId = special.SpecialCategoryId,
            Title = special.Title,
            Description = special.Description,
            CategoryName = special.Category?.Name ?? string.Empty,
            CategoryIcon = special.Category?.Icon,
            StartDate = special.StartDate.ToString("yyyy-MM-dd", null),
            StartTime = special.StartTime.ToString("HH:mm", null),
            EndTime = special.EndTime?.ToString("HH:mm", null),
            EndDate = special.EndDate?.ToString("yyyy-MM-dd", null),
            IsRecurring = special.IsRecurring,
            IsActive = special.IsActive
        };
    }

    private static VenueEntity MapToVenueEntity(CreateVenue createVenue)
    {
        Point? location = null;
        if (createVenue.Latitude.HasValue && createVenue.Longitude.HasValue)
        {
            location = new Point(createVenue.Longitude.Value, createVenue.Latitude.Value) { SRID = 4326 };
        }

        return new VenueEntity
        {
            CategoryId = createVenue.CategoryId,
            Name = createVenue.Name,
            Description = createVenue.Description,
            PhoneNumber = createVenue.PhoneNumber,
            Website = createVenue.Website,
            Email = createVenue.Email,
            ProfileImage = createVenue.ProfileImage,
            StreetAddress = createVenue.StreetAddress,
            SecondaryAddress = createVenue.SecondaryAddress,
            Locality = createVenue.Locality,
            Region = createVenue.Region,
            PostalCode = createVenue.PostalCode,
            Country = createVenue.Country,
            Location = location,
            IsActive = createVenue.IsActive
        };
    }

    private static void UpdateVenueEntity(VenueEntity venue, UpdateVenue updateVenue)
    {
        venue.CategoryId = updateVenue.CategoryId;
        venue.Name = updateVenue.Name;
        venue.Description = updateVenue.Description;
        venue.PhoneNumber = updateVenue.PhoneNumber;
        venue.Website = updateVenue.Website;
        venue.Email = updateVenue.Email;
        venue.ProfileImage = updateVenue.ProfileImage;
        venue.StreetAddress = updateVenue.StreetAddress;
        venue.SecondaryAddress = updateVenue.SecondaryAddress;
        venue.Locality = updateVenue.Locality;
        venue.Region = updateVenue.Region;
        venue.PostalCode = updateVenue.PostalCode;
        venue.Country = updateVenue.Country;
        venue.IsActive = updateVenue.IsActive;

        if (updateVenue.Latitude.HasValue && updateVenue.Longitude.HasValue)
        {
            venue.Location = new Point(updateVenue.Longitude.Value, updateVenue.Latitude.Value) { SRID = 4326 };
        }
        else
        {
            venue.Location = null;
        }
    }

    #endregion

    #region Azure Maps Enhanced Location Methods

    /// <summary>
    /// Geocodes an address and creates/updates venue location using Azure Maps
    /// </summary>
    public async Task<ApiResponse<GeocodeResult?>> GeocodeVenueAddressAsync(long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            var venue = await _venueRepository.GetByIdAsync(venueId, cancellationToken);
            if (venue == null)
            {
                return ApiResponse<GeocodeResult?>.ErrorResult($"Venue with ID {venueId} not found.");
            }

            // Build address string from venue data
            var addressParts = new List<string>();
            if (!string.IsNullOrEmpty(venue.StreetAddress)) addressParts.Add(venue.StreetAddress);
            if (!string.IsNullOrEmpty(venue.SecondaryAddress)) addressParts.Add(venue.SecondaryAddress);
            if (!string.IsNullOrEmpty(venue.Locality)) addressParts.Add(venue.Locality);
            if (!string.IsNullOrEmpty(venue.Region)) addressParts.Add(venue.Region);
            if (!string.IsNullOrEmpty(venue.PostalCode)) addressParts.Add(venue.PostalCode);
            if (!string.IsNullOrEmpty(venue.Country)) addressParts.Add(venue.Country);

            var address = string.Join(", ", addressParts);
            if (string.IsNullOrEmpty(address))
            {
                return ApiResponse<GeocodeResult?>.ErrorResult("Venue address is not complete enough for geocoding.");
            }

            // Use Azure Maps to geocode the address
            var geocodeResult = await _azureMapsService.GeocodeAddressAsync(address, cancellationToken);
            if (geocodeResult != null)
            {
                // Update venue location with geocoded coordinates
                venue.Location = new Point(geocodeResult.Longitude, geocodeResult.Latitude) { SRID = 4326 };
                await _venueRepository.UpdateAsync(venue, cancellationToken);
                await _venueRepository.SaveChangesAsync(cancellationToken);
            }

            return ApiResponse<GeocodeResult?>.SuccessResult(geocodeResult);
        }
        catch (Exception ex)
        {
            return ApiResponse<GeocodeResult?>.ErrorResult($"Error geocoding venue address: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets enhanced location information for a venue using Azure Maps reverse geocoding
    /// </summary>
    public async Task<ApiResponse<ReverseGeocodeResult?>> GetVenueLocationDetailsAsync(long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            var venue = await _venueRepository.GetByIdAsync(venueId, cancellationToken);
            if (venue == null)
            {
                return ApiResponse<ReverseGeocodeResult?>.ErrorResult($"Venue with ID {venueId} not found.");
            }

            if (venue.Location == null)
            {
                return ApiResponse<ReverseGeocodeResult?>.ErrorResult("Venue does not have location coordinates.");
            }

            // Use Azure Maps to get detailed location information
            var locationDetails = await _azureMapsService.ReverseGeocodeAsync(
                venue.Location.Y, 
                venue.Location.X, 
                cancellationToken);

            return ApiResponse<ReverseGeocodeResult?>.SuccessResult(locationDetails);
        }
        catch (Exception ex)
        {
            return ApiResponse<ReverseGeocodeResult?>.ErrorResult($"Error getting venue location details: {ex.Message}");
        }
    }

    /// <summary>
    /// Finds nearby points of interest using Azure Maps and venue location
    /// </summary>
    public async Task<ApiResponse<IEnumerable<PointOfInterest>>> GetNearbyPointsOfInterestAsync(
        long venueId, 
        string? category = null, 
        int radiusInMeters = 1000, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var venue = await _venueRepository.GetByIdAsync(venueId, cancellationToken);
            if (venue == null)
            {
                return ApiResponse<IEnumerable<PointOfInterest>>.ErrorResult($"Venue with ID {venueId} not found.");
            }

            if (venue.Location == null)
            {
                return ApiResponse<IEnumerable<PointOfInterest>>.ErrorResult("Venue does not have location coordinates.");
            }

            // Use Azure Maps to find nearby POIs
            var nearbyPOIs = await _azureMapsService.SearchNearbyAsync(
                venue.Location.Y, 
                venue.Location.X, 
                category, 
                radiusInMeters, 
                cancellationToken);

            return ApiResponse<IEnumerable<PointOfInterest>>.SuccessResult(nearbyPOIs);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<PointOfInterest>>.ErrorResult($"Error finding nearby points of interest: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets timezone information for a venue using Azure Maps
    /// </summary>
    public async Task<ApiResponse<TimeZoneInfo?>> GetVenueTimeZoneAsync(long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            var venue = await _venueRepository.GetByIdAsync(venueId, cancellationToken);
            if (venue == null)
            {
                return ApiResponse<TimeZoneInfo?>.ErrorResult($"Venue with ID {venueId} not found.");
            }

            if (venue.Location == null)
            {
                return ApiResponse<TimeZoneInfo?>.ErrorResult("Venue does not have location coordinates.");
            }

            // Use Azure Maps to get timezone information
            var timeZone = await _azureMapsService.GetTimeZoneAsync(
                venue.Location.Y, 
                venue.Location.X, 
                cancellationToken);

            return ApiResponse<TimeZoneInfo?>.SuccessResult(timeZone);
        }
        catch (Exception ex)
        {
            return ApiResponse<TimeZoneInfo?>.ErrorResult($"Error getting venue timezone: {ex.Message}");
        }
    }

    /// <summary>
    /// Enhanced venue search that combines PostGIS database search with Azure Maps POI data
    /// </summary>
    public async Task<ApiResponse<EnhancedVenueSearchResult>> SearchVenuesWithPOIDataAsync(
        VenueSearch search, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // First, get venues from our database using PostGIS
            var databaseResults = await SearchVenuesAsync(search, cancellationToken);
            if (!databaseResults.Success)
            {
                return ApiResponse<EnhancedVenueSearchResult>.ErrorResult(databaseResults.Message ?? "Database search failed");
            }

            var enhancedResult = new EnhancedVenueSearchResult
            {
                DatabaseResults = new PagedResponse<VenueSummary>
                {
                    Items = databaseResults.Data!.Items.Select(ConvertVenueSummaryToVenueSummary),
                    TotalCount = databaseResults.Data!.TotalCount,
                    PageNumber = databaseResults.Data!.PageNumber,
                    PageSize = databaseResults.Data!.PageSize
                },
                AzureMapsPOIs = new List<PointOfInterest>()
            };

            // If we have a location, also search Azure Maps for nearby POIs
            if (search.Latitude.HasValue && search.Longitude.HasValue)
            {
                var azurePOIs = await _azureMapsService.SearchNearbyAsync(
                    search.Latitude.Value,
                    search.Longitude.Value,
                    search.SearchTerm,
                    (int)(search.RadiusInMeters ?? 5000),
                    cancellationToken);

                enhancedResult.AzureMapsPOIs = azurePOIs.ToList();
            }

            return ApiResponse<EnhancedVenueSearchResult>.SuccessResult(enhancedResult);
        }
        catch (Exception ex)
        {
            return ApiResponse<EnhancedVenueSearchResult>.ErrorResult($"Error performing enhanced venue search: {ex.Message}");
        }
    }

    private static VenueSummary ConvertVenueSummaryToVenueSummary(VenueSummary dto)
    {
        return new VenueSummary
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            PhoneNumber = dto.PhoneNumber,
            Website = dto.Website,
            Email = dto.Email,
            ProfileImage = dto.ProfileImage,
            StreetAddress = dto.StreetAddress,
            SecondaryAddress = dto.SecondaryAddress,
            Locality = dto.Locality,
            Region = dto.Region,
            PostalCode = dto.PostalCode,
            Country = dto.Country,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            CategoryIcon = dto.CategoryIcon,
            DistanceInMeters = dto.DistanceInMeters
        };
    }

    #endregion
}
