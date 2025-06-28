using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Common.Models.Search;
using Application.Common.Models.Special;
using Application.Domain.Entities;
using Application.Domain.Helpers;
using NetTopologySuite.Geometries;
using NodaTime;
using NodaTime.Text;

namespace Application.Infrastructure.Services;

/// <summary>
/// Service implementation for special operations
/// </summary>
public class SpecialService : ISpecialService
{
    private readonly ISpecialRepository _specialRepository;
    private readonly ISpecialCategoryRepository _specialCategoryRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IAzureMapsService _azureMapsService;
    private readonly IClock _clock;

    public SpecialService(
        ISpecialRepository specialRepository,
        ISpecialCategoryRepository specialCategoryRepository,
        IVenueRepository venueRepository,
        IAzureMapsService azureMapsService,
        IClock clock)
    {
        _specialRepository = specialRepository ?? throw new ArgumentNullException(nameof(specialRepository));
        _specialCategoryRepository = specialCategoryRepository ?? throw new ArgumentNullException(nameof(specialCategoryRepository));
        _venueRepository = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
        _azureMapsService = azureMapsService ?? throw new ArgumentNullException(nameof(azureMapsService));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task<ApiResponse<Special?>> GetSpecialByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var special = await _specialRepository.GetSpecialWithDetailsAsync(id, cancellationToken);
            if (special == null)
            {
                return ApiResponse<Special?>.ErrorResult($"Special with ID {id} not found.");
            }

            var specialDto = MapToSpecial(special);
            return ApiResponse<Special?>.SuccessResult(specialDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<Special?>.ErrorResult($"Error retrieving special: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SpecialSummary>>> GetAllSpecialsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var specials = await _specialRepository.GetAllAsync(cancellationToken);
            var specialDtos = specials.Select(MapToSpecialSummary);
            return ApiResponse<IEnumerable<SpecialSummary>>.SuccessResult(specialDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SpecialSummary>>.ErrorResult($"Error retrieving specials: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Special>> CreateSpecialAsync(CreateSpecial createSpecial, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify venue exists
            var venue = await _venueRepository.GetByIdAsync(createSpecial.VenueId, cancellationToken);
            if (venue == null)
            {
                return ApiResponse<Special>.ErrorResult($"Venue with ID {createSpecial.VenueId} not found.");
            }

            // Verify category exists
            var category = await _specialCategoryRepository.GetByIdAsync(createSpecial.SpecialCategoryId, cancellationToken);
            if (category == null)
            {
                return ApiResponse<Special>.ErrorResult($"Special category with ID {createSpecial.SpecialCategoryId} not found.");
            }

            var special = MapToSpecialEntity(createSpecial);
            var createdSpecial = await _specialRepository.AddAsync(special, cancellationToken);
            await _specialRepository.SaveChangesAsync(cancellationToken);

            var specialWithDetails = await _specialRepository.GetSpecialWithDetailsAsync(createdSpecial.Id, cancellationToken);
            var specialDto = MapToSpecial(specialWithDetails!);

            return ApiResponse<Special>.SuccessResult(specialDto, "Special created successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<Special>.ErrorResult($"Error creating special: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Special?>> UpdateSpecialAsync(long id, UpdateSpecial updateSpecial, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingSpecial = await _specialRepository.GetByIdAsync(id, cancellationToken);
            if (existingSpecial == null)
            {
                return ApiResponse<Special?>.ErrorResult($"Special with ID {id} not found.");
            }

            // Verify venue exists if changed
            if (updateSpecial.VenueId != existingSpecial.VenueId)
            {
                var venue = await _venueRepository.GetByIdAsync(updateSpecial.VenueId, cancellationToken);
                if (venue == null)
                {
                    return ApiResponse<Special?>.ErrorResult($"Venue with ID {updateSpecial.VenueId} not found.");
                }
            }

            // Verify category exists if changed
            if (updateSpecial.SpecialCategoryId != existingSpecial.SpecialCategoryId)
            {
                var category = await _specialCategoryRepository.GetByIdAsync(updateSpecial.SpecialCategoryId, cancellationToken);
                if (category == null)
                {
                    return ApiResponse<Special?>.ErrorResult($"Special category with ID {updateSpecial.SpecialCategoryId} not found.");
                }
            }

            // Update entity
            UpdateSpecialEntity(existingSpecial, updateSpecial);
            await _specialRepository.UpdateAsync(existingSpecial, cancellationToken);
            await _specialRepository.SaveChangesAsync(cancellationToken);

            var specialWithDetails = await _specialRepository.GetSpecialWithDetailsAsync(id, cancellationToken);
            var specialDto = MapToSpecial(specialWithDetails!);

            return ApiResponse<Special?>.SuccessResult(specialDto, "Special updated successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<Special?>.ErrorResult($"Error updating special: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteSpecialAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _specialRepository.DeleteAsync(id, cancellationToken);
            await _specialRepository.SaveChangesAsync(cancellationToken);
            return ApiResponse<bool>.SuccessResult(true, "Special deleted successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResult($"Error deleting special: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SpecialSummary>>> GetActiveSpecialsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var allActiveSpecials = await _specialRepository.GetActiveSpecialsAsync(cancellationToken);
            var currentDateTime = _clock.GetCurrentInstant().InZone(DateTimeZone.Utc).LocalDateTime;
            
            // Filter by CRON schedule to get truly active specials
            var currentlyActiveSpecials = allActiveSpecials
                .Where(special => CronScheduleEvaluator.IsSpecialCurrentlyActive(special, currentDateTime))
                .ToList();
            
            var specialDtos = currentlyActiveSpecials.Select(MapToSpecialSummary);
            return ApiResponse<IEnumerable<SpecialSummary>>.SuccessResult(specialDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SpecialSummary>>.ErrorResult($"Error retrieving active specials: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SpecialSummary>>> GetSpecialsByVenueAsync(long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            var specials = await _specialRepository.GetSpecialsByVenueAsync(venueId, cancellationToken);
            var specialDtos = specials.Select(MapToSpecialSummary);
            return ApiResponse<IEnumerable<SpecialSummary>>.SuccessResult(specialDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SpecialSummary>>.ErrorResult($"Error retrieving specials for venue: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SpecialSummary>>> GetActiveSpecialsByVenueAsync(long venueId, CancellationToken cancellationToken = default)
    {
        try
        {
            var allActiveSpecials = await _specialRepository.GetActiveSpecialsByVenueAsync(venueId, cancellationToken);
            var currentDateTime = _clock.GetCurrentInstant().InZone(DateTimeZone.Utc).LocalDateTime;
            
            // Filter by CRON schedule to get truly active specials
            var currentlyActiveSpecials = allActiveSpecials
                .Where(special => CronScheduleEvaluator.IsSpecialCurrentlyActive(special, currentDateTime))
                .ToList();
            
            var specialDtos = currentlyActiveSpecials.Select(MapToSpecialSummary);
            return ApiResponse<IEnumerable<SpecialSummary>>.SuccessResult(specialDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SpecialSummary>>.ErrorResult($"Error retrieving active specials for venue: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SpecialSummary>>> GetSpecialsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var specials = await _specialRepository.GetSpecialsByCategoryAsync(categoryId, cancellationToken);
            var specialDtos = specials.Select(MapToSpecialSummary);
            return ApiResponse<IEnumerable<SpecialSummary>>.SuccessResult(specialDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SpecialSummary>>.ErrorResult($"Error retrieving specials by category: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SpecialSummary>>> GetActiveSpecialsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var specials = await _specialRepository.GetActiveSpecialsByCategoryAsync(categoryId, cancellationToken);
            var specialDtos = specials.Select(MapToSpecialSummary);
            return ApiResponse<IEnumerable<SpecialSummary>>.SuccessResult(specialDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SpecialSummary>>.ErrorResult($"Error retrieving active specials by category: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SpecialSummary>>> GetRecurringSpecialsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var specials = await _specialRepository.GetRecurringSpecialsAsync(cancellationToken);
            var specialDtos = specials.Select(MapToSpecialSummary);
            return ApiResponse<IEnumerable<SpecialSummary>>.SuccessResult(specialDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SpecialSummary>>.ErrorResult($"Error retrieving recurring specials: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SpecialSummary>>> GetSpecialsActiveNowAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var now = _clock.GetCurrentInstant().InUtc().LocalDateTime;
            var specials = await _specialRepository.GetSpecialsActiveAtTimeAsync(now, cancellationToken);
            var specialDtos = specials.Select(MapToSpecialSummary);
            return ApiResponse<IEnumerable<SpecialSummary>>.SuccessResult(specialDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SpecialSummary>>.ErrorResult($"Error retrieving active specials: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SpecialSummary>>> GetActiveSpecialsNearLocationAsync(double latitude, double longitude, double radiusInMeters = 5000, CancellationToken cancellationToken = default)
    {
        try
        {
            var specials = await _specialRepository.GetActiveSpecialsNearLocationAsync(latitude, longitude, radiusInMeters, cancellationToken);
            var specialDtos = specials.Select(MapToSpecialSummary);
            return ApiResponse<IEnumerable<SpecialSummary>>.SuccessResult(specialDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SpecialSummary>>.ErrorResult($"Error retrieving specials near location: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PagedResponse<SpecialSummary>>> SearchSpecialsAsync(SpecialSearch search, CancellationToken cancellationToken = default)
    {
        try
        {
            // Map the search DTO to the repository search parameters
            var searchResults = await _specialRepository.SearchSpecialsAsync(
                search.SearchTerm,
                search.CategoryId,
                search.VenueId,
                !string.IsNullOrEmpty(search.StartDate) ? LocalDatePattern.Iso.Parse(search.StartDate).GetValueOrThrow() : null,
                !string.IsNullOrEmpty(search.EndDate) ? LocalDatePattern.Iso.Parse(search.EndDate).GetValueOrThrow() : null,
                search.ActiveOnly,
                cancellationToken);

            var specialDtos = searchResults.Select(MapToSpecialSummary).ToList();
            
            // Simple pagination (in a real implementation, you'd want the repository to handle this)
            var totalCount = specialDtos.Count;
            var pagedItems = specialDtos
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize);

            var pagedResponse = new PagedResponse<SpecialSummary>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = search.PageNumber,
                PageSize = search.PageSize
            };

            return ApiResponse<PagedResponse<SpecialSummary>>.SuccessResult(pagedResponse);
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResponse<SpecialSummary>>.ErrorResult($"Error searching specials: {ex.Message}");
        }
    }

    // Enhanced search operations - returns venues with categorized specials
    public async Task<ApiResponse<PagedResponse<VenueWithCategorizedSpecials>>> SearchVenuesWithSpecialsAsync(
        EnhancedSpecialSearch searchRequest, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Parse date and time, use defaults if not provided
            var searchDate = !string.IsNullOrEmpty(searchRequest.Date) 
                ? LocalDatePattern.Iso.Parse(searchRequest.Date).GetValueOrThrow()
                : _clock.GetCurrentInstant().InUtc().Date;
            
            var searchTime = !string.IsNullOrEmpty(searchRequest.Time) 
                ? LocalTimePattern.CreateWithInvariantCulture("HH:mm").Parse(searchRequest.Time).GetValueOrThrow()
                : _clock.GetCurrentInstant().InUtc().TimeOfDay;

            // Resolve location coordinates if address is provided
            double? resolvedLat = searchRequest.Latitude;
            double? resolvedLng = searchRequest.Longitude;
            
            if (!string.IsNullOrEmpty(searchRequest.Address) && (!resolvedLat.HasValue || !resolvedLng.HasValue))
            {
                var geocodeResult = await _azureMapsService.GeocodeAddressAsync(searchRequest.Address, cancellationToken);
                if (geocodeResult != null)
                {
                    resolvedLat = geocodeResult.Latitude;
                    resolvedLng = geocodeResult.Longitude;
                }
            }

            // Get venues with active specials in the area
            var venuesWithSpecials = await GetVenuesWithSpecialsInArea(
                resolvedLat,
                resolvedLng,
                searchRequest.RadiusInMeters ?? 5000,
                searchDate,
                searchTime,
                searchRequest.SearchTerm,
                searchRequest.ActiveOnly,
                searchRequest.CurrentlyRunning,
                cancellationToken);

            // Apply sorting
            venuesWithSpecials = ApplySorting(venuesWithSpecials, searchRequest.SortBy, searchRequest.SortOrder);

            // Apply pagination
            var totalCount = venuesWithSpecials.Count;
            var pagedItems = venuesWithSpecials
                .Skip((searchRequest.PageNumber - 1) * searchRequest.PageSize)
                .Take(searchRequest.PageSize);

            var pagedResponse = new PagedResponse<VenueWithCategorizedSpecials>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = searchRequest.PageNumber,
                PageSize = searchRequest.PageSize
            };

            return ApiResponse<PagedResponse<VenueWithCategorizedSpecials>>.SuccessResult(pagedResponse);
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResponse<VenueWithCategorizedSpecials>>.ErrorResult($"Error searching venues with specials: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SpecialCategory>>> GetSpecialCategoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _specialCategoryRepository.GetAllAsync(cancellationToken);
            var categoryDtos = categories.Select(MapToSpecialCategory);
            return ApiResponse<IEnumerable<SpecialCategory>>.SuccessResult(categoryDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SpecialCategory>>.ErrorResult($"Error retrieving special categories: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SpecialCategory?>> GetSpecialCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _specialCategoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                return ApiResponse<SpecialCategory?>.ErrorResult($"Special category with ID {id} not found.");
            }

            var categoryDto = MapToSpecialCategory(category);
            return ApiResponse<SpecialCategory?>.SuccessResult(categoryDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<SpecialCategory?>.ErrorResult($"Error retrieving special category: {ex.Message}");
        }
    }

    #region Private Helper Methods

    private async Task<List<VenueWithCategorizedSpecials>> GetVenuesWithSpecialsInArea(
        double? latitude,
        double? longitude,
        double radiusInMeters,
        LocalDate searchDate,
        LocalTime searchTime,
        string? searchTerm,
        bool activeOnly,
        bool currentlyRunning,
        CancellationToken cancellationToken)
    {
        var venues = new List<VenueWithCategorizedSpecials>();

        // If no location provided, get all venues, otherwise use geospatial search
        IEnumerable<VenueEntity> venueEntities;
        if (latitude.HasValue && longitude.HasValue)
        {
            venueEntities = await _venueRepository.GetVenuesInRadiusAsync(
                latitude.Value, longitude.Value, radiusInMeters, cancellationToken);
        }
        else
        {
            venueEntities = await _venueRepository.GetAllAsync(cancellationToken);
        }

        // Apply search term filter if provided
        if (!string.IsNullOrEmpty(searchTerm))
        {
            venueEntities = venueEntities.Where(v => 
                v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(v.Description) && v.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        foreach (var venue in venueEntities)
        {
            // Get specials for this venue based on active filter
            IEnumerable<SpecialEntity> allSpecials;
            if (activeOnly)
            {
                allSpecials = await _specialRepository.GetActiveSpecialsByVenueAsync(venue.Id, cancellationToken);
            }
            else
            {
                allSpecials = await _specialRepository.GetSpecialsByVenueAsync(venue.Id, cancellationToken);
                allSpecials = allSpecials.Where(s => s.IsActive); // Still filter by IsActive flag
            }
            
            // Filter specials by the specified date and time if currentlyRunning is true
            IEnumerable<SpecialEntity> specials;
            if (currentlyRunning)
            {
                var searchDateTime = searchDate.At(searchTime);
                specials = allSpecials.Where(s => CronScheduleEvaluator.IsSpecialCurrentlyActive(s, searchDateTime));
            }
            else
            {
                // When not filtering for currently running, filter out past one-time specials
                var currentDate = searchDate;
                specials = allSpecials.Where(s => 
                {
                    // Always include recurring specials (they don't expire based on date alone)
                    if (s.IsRecurring)
                        return true;
                    
                    // For one-time specials, only include future or current ones
                    if (s.EndDate.HasValue)
                    {
                        return s.EndDate.Value >= currentDate;
                    }
                    else
                    {
                        return s.StartDate >= currentDate;
                    }
                });
            }

            if (!specials.Any() && activeOnly && currentlyRunning)
                continue;

            // Categorize specials by type (based on category names)
            var categorizedSpecials = new CategorizedSpecials
            {
                Food = specials.Where(s => IsSpecialCategory(s.Category?.Name, "food")).Select(MapToSpecialSummary),
                Drink = specials.Where(s => IsSpecialCategory(s.Category?.Name, "drink")).Select(MapToSpecialSummary),
                Entertainment = specials.Where(s => IsSpecialCategory(s.Category?.Name, "entertainment")).Select(MapToSpecialSummary)
            };

            var venueWithSpecials = new VenueWithCategorizedSpecials
            {
                Id = venue.Id,
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
                CategoryId = venue.CategoryId,
                CategoryName = venue.Category?.Name ?? string.Empty,
                CategoryIcon = venue.Category?.Icon,
                DistanceInMeters = latitude.HasValue && longitude.HasValue && venue.Location != null
                    ? CalculateDistance(latitude.Value, longitude.Value, venue.Location.Y, venue.Location.X)
                    : null,
                Specials = categorizedSpecials,
                TotalSpecialCount = specials.Count()
            };

            venues.Add(venueWithSpecials);
        }

        return venues;
    }

    private static bool IsSpecialActiveAtTime(SpecialEntity special, LocalDateTime dateTime)
    {
        // Use the consistent CRON schedule evaluator for all specials
        return CronScheduleEvaluator.IsSpecialCurrentlyActive(special, dateTime);
    }

    private static bool IsSpecialCategory(string? categoryName, string targetType)
    {
        if (string.IsNullOrEmpty(categoryName))
            return false;

        return targetType.ToLowerInvariant() switch
        {
            "food" => categoryName.Contains("food", StringComparison.OrdinalIgnoreCase) ||
                      categoryName.Contains("meal", StringComparison.OrdinalIgnoreCase) ||
                      categoryName.Contains("dining", StringComparison.OrdinalIgnoreCase) ||
                      categoryName.Contains("appetizer", StringComparison.OrdinalIgnoreCase) ||
                      categoryName.Contains("entree", StringComparison.OrdinalIgnoreCase) ||
                      categoryName.Contains("dessert", StringComparison.OrdinalIgnoreCase),
            "drink" => categoryName.Contains("drink", StringComparison.OrdinalIgnoreCase) ||
                       categoryName.Contains("beverage", StringComparison.OrdinalIgnoreCase) ||
                       categoryName.Contains("cocktail", StringComparison.OrdinalIgnoreCase) ||
                       categoryName.Contains("beer", StringComparison.OrdinalIgnoreCase) ||
                       categoryName.Contains("wine", StringComparison.OrdinalIgnoreCase) ||
                       categoryName.Contains("alcohol", StringComparison.OrdinalIgnoreCase),
            "entertainment" => categoryName.Contains("entertainment", StringComparison.OrdinalIgnoreCase) ||
                               categoryName.Contains("music", StringComparison.OrdinalIgnoreCase) ||
                               categoryName.Contains("event", StringComparison.OrdinalIgnoreCase) ||
                               categoryName.Contains("show", StringComparison.OrdinalIgnoreCase) ||
                               categoryName.Contains("performance", StringComparison.OrdinalIgnoreCase) ||
                               categoryName.Contains("karaoke", StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // Earth's radius in meters
        var lat1Rad = lat1 * Math.PI / 180;
        var lat2Rad = lat2 * Math.PI / 180;
        var deltaLatRad = (lat2 - lat1) * Math.PI / 180;
        var deltaLonRad = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    #endregion

    #region Private Mapping Methods

    private static Special MapToSpecial(SpecialEntity special)
    {
        return new Special
        {
            Id = special.Id,
            VenueId = special.VenueId,
            SpecialCategoryId = special.SpecialCategoryId,
            Title = special.Title,
            Description = special.Description,
            StartDate = special.StartDate.ToString("yyyy-MM-dd", null),
            StartTime = special.StartTime.ToString("HH:mm", null),
            EndTime = special.EndTime?.ToString("HH:mm", null),
            EndDate = special.EndDate?.ToString("yyyy-MM-dd", null),
            IsRecurring = special.IsRecurring,
            CronSchedule = special.CronSchedule,
            IsActive = special.IsActive,
            Venue = special.Venue != null ? MapToVenueSpecial(special.Venue) : null,
            Category = special.Category != null ? MapToSpecialCategory(special.Category) : null
        };
    }

    private static SpecialSummary MapToSpecialSummary(SpecialEntity special)
    {
        return new SpecialSummary
        {
            Id = special.Id,
            VenueId = special.VenueId,
            VenueName = special.Venue?.Name ?? string.Empty,
            SpecialCategoryId = special.SpecialCategoryId,
            CategoryName = special.Category?.Name ?? string.Empty,
            CategoryIcon = special.Category?.Icon,
            Title = special.Title,
            Description = special.Description,
            StartDate = special.StartDate.ToString("yyyy-MM-dd", null),
            StartTime = special.StartTime.ToString("HH:mm", null),
            EndTime = special.EndTime?.ToString("HH:mm", null),
            EndDate = special.EndDate?.ToString("yyyy-MM-dd", null),
            IsRecurring = special.IsRecurring,
            CronSchedule = special.CronSchedule,
            IsActive = special.IsActive
        };
    }

    private static SpecialCategory MapToSpecialCategory(SpecialCategoryEntity category)
    {
        return new SpecialCategory
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Icon = category.Icon,
            SortOrder = category.SortOrder
        };
    }

    private static VenueSpecial MapToVenueSpecial(VenueEntity venue)
    {
        return new VenueSpecial
        {
            Id = venue.Id,
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
            CategoryId = venue.CategoryId,
            CategoryName = venue.Category?.Name ?? string.Empty,
            CategoryIcon = venue.Category?.Icon
        };
    }

    private SpecialEntity MapToSpecialEntity(CreateSpecial dto)
    {
        return new SpecialEntity
        {
            VenueId = dto.VenueId,
            SpecialCategoryId = dto.SpecialCategoryId,
            Title = dto.Title,
            Description = dto.Description,
            StartDate = LocalDatePattern.Iso.Parse(dto.StartDate).GetValueOrThrow(),
            StartTime = LocalTimePattern.CreateWithInvariantCulture("HH:mm").Parse(dto.StartTime).GetValueOrThrow(),
            EndTime = !string.IsNullOrEmpty(dto.EndTime) 
                ? LocalTimePattern.CreateWithInvariantCulture("HH:mm").Parse(dto.EndTime).GetValueOrThrow() 
                : null,
            EndDate = !string.IsNullOrEmpty(dto.EndDate) 
                ? LocalDatePattern.Iso.Parse(dto.EndDate).GetValueOrThrow() 
                : null,
            IsRecurring = dto.IsRecurring,
            CronSchedule = dto.CronSchedule,
            IsActive = dto.IsActive
        };
    }

    private void UpdateSpecialEntity(SpecialEntity entity, UpdateSpecial dto)
    {
        entity.VenueId = dto.VenueId;
        entity.SpecialCategoryId = dto.SpecialCategoryId;
        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.StartDate = LocalDatePattern.Iso.Parse(dto.StartDate).GetValueOrThrow();
        entity.StartTime = LocalTimePattern.CreateWithInvariantCulture("HH:mm").Parse(dto.StartTime).GetValueOrThrow();
        entity.EndTime = !string.IsNullOrEmpty(dto.EndTime) 
            ? LocalTimePattern.CreateWithInvariantCulture("HH:mm").Parse(dto.EndTime).GetValueOrThrow() 
            : null;
        entity.EndDate = !string.IsNullOrEmpty(dto.EndDate) 
            ? LocalDatePattern.Iso.Parse(dto.EndDate).GetValueOrThrow() 
            : null;
        entity.IsRecurring = dto.IsRecurring;
        entity.CronSchedule = dto.CronSchedule;
        entity.IsActive = dto.IsActive;
    }

    private static List<VenueWithCategorizedSpecials> ApplySorting(
        List<VenueWithCategorizedSpecials> venues, 
        string? sortBy, 
        string? sortOrder)
    {
        if (venues == null || !venues.Any())
            return venues ?? new List<VenueWithCategorizedSpecials>();

        var isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy?.ToLowerInvariant() switch
        {
            "name" => isDescending 
                ? venues.OrderByDescending(v => v.Name).ToList()
                : venues.OrderBy(v => v.Name).ToList(),
                
            "special_count" => isDescending
                ? venues.OrderByDescending(v => v.TotalSpecialCount).ToList()
                : venues.OrderBy(v => v.TotalSpecialCount).ToList(),
                
            "distance" or _ => isDescending
                ? venues.OrderByDescending(v => v.DistanceInMeters ?? double.MaxValue).ToList()
                : venues.OrderBy(v => v.DistanceInMeters ?? double.MaxValue).ToList()
        };
    }

    #endregion
}
