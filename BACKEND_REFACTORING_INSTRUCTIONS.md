# 🎯 Backend Refactoring Instructions for GitHub Copilot

## Overview
This document provides step-by-step instructions for refactoring the Pulse backend to follow SOLID and DRY principles, improve controller organization, and create better separation of concerns.

## 🎯 Refactoring Goals

### Primary Objectives
1. **Follow SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
2. **Implement DRY (Don't Repeat Yourself)**: Eliminate code duplication through helpers, utilities, and base classes
3. **Improve Controller Organization**: Split BackofficeController into logical domain controllers
4. **Enhance Maintainability**: Make the codebase easier to extend and modify
5. **Standardize Patterns**: Create consistent patterns across all controllers

### Secondary Objectives
- Improve error handling consistency
- Standardize logging patterns
- Reduce boilerplate code
- Better separation of concerns
- Easier testing and mocking

## 📋 Refactoring Phases

### Phase 1: Infrastructure Enhancement ✅ COMPLETED
**Status: DONE** - Enhanced constants, helpers, and utilities have been created.

**What was created:**
- `HttpStatusConstants.cs` - HTTP status code constants
- `DatabaseConstants.cs` - Database-related constants
- `ControllerAuthorizationHelper.cs` - Authorization helper methods
- `ControllerBaseHelper.cs` - Common controller functionality
- `SearchParameterHelper.cs` - Search parameter validation
- `PermissionValidationHelper.cs` - Permission checking patterns
- `BaseApiController.cs` - Base controller class

### Phase 2: Controller Refactoring 🔄 IN PROGRESS

#### Step 1: Create New Domain-Specific Controllers

**A. VenueManagementController**
- **Purpose**: Handle venue CRUD operations and management
- **Routes**: All venue-related routes from ApiRoutes.Venues
- **Inherits**: BaseApiController
- **Endpoints to move from BackofficeController**:
  - `GET /api/venues/my` (GetMyVenues)
  - `POST /api/venues` (CreateVenue)  
  - `PUT /api/venues/{id}` (UpdateVenue)
  - `DELETE /api/venues/{id}` (DeleteVenue)

**B. VenuePermissionController**
- **Purpose**: Handle venue permissions and invitations
- **Routes**: All permission-related routes from ApiRoutes.Permissions
- **Inherits**: BaseApiController
- **Endpoints to move from BackofficeController**:
  - `GET /api/venues/{venueId}/permissions` (GetVenuePermissions)
  - `GET /api/permissions/my` (GetMyPermissions)
  - `PUT /api/permissions/{permissionId}` (UpdatePermission)

**C. VenueInvitationController**  
- **Purpose**: Handle venue invitations
- **Routes**: All invitation-related routes from ApiRoutes.Invitations
- **Inherits**: BaseApiController
- **Endpoints to move from BackofficeController**:
  - `GET /api/venues/{venueId}/invitations` (GetVenueInvitations)
  - `POST /api/invitations` (SendInvitation)
  - `GET /api/invitations/my` (GetMyInvitations)
  - `POST /api/invitations/{id}/accept` (AcceptInvitation)
  - `POST /api/invitations/{id}/decline` (DeclineInvitation)
  - `DELETE /api/invitations/{id}` (CancelInvitation)

**D. SpecialManagementController**
- **Purpose**: Handle special CRUD operations and management
- **Routes**: Special management routes from ApiRoutes.Specials
- **Inherits**: BaseApiController
- **Endpoints to move from BackofficeController**:
  - `GET /api/specials/my` (GetMySpecials)
  - `POST /api/venues/{venueId}/specials` (CreateSpecial)
  - `PUT /api/specials/{id}` (UpdateSpecial)
  - `DELETE /api/specials/{id}` (DeleteSpecial)

**E. UserManagementController**
- **Purpose**: Handle user synchronization and profiles
- **Routes**: User-related routes from ApiRoutes.Users
- **Inherits**: BaseApiController
- **Endpoints to move from BackofficeController**:
  - `POST /api/users/sync` (SyncUser)
  - `GET /api/permission-types` (GetPermissionTypes)

#### Step 2: Refactor Existing Controllers

**A. VenuesController Improvements**
- Inherit from BaseApiController
- Use individual route attributes (no class-level route)
- Apply SearchParameterHelper for validation
- Use PermissionValidationHelper for authorization
- Standardize error handling

**B. SpecialsController Improvements**
- Inherit from BaseApiController  
- Use individual route attributes
- Apply SearchParameterHelper for validation
- Standardize response patterns

**C. LocationController Improvements**
- Inherit from BaseApiController
- Use validation helpers
- Standardize error responses

#### Step 3: Eliminate BackofficeController
- **Goal**: Remove the monolithic BackofficeController entirely
- **Process**: Move all endpoints to appropriate domain controllers
- **Result**: Better separation of concerns and easier maintenance

### Phase 3: Service Layer Enhancement 🔄 NEXT

#### Step 1: Service Interface Improvements
- Add consistent cancellation token support
- Standardize return types (ApiResponse<T>)
- Add validation methods to service interfaces
- Create service base interfaces for common operations

#### Step 2: Service Implementation Enhancements
- Extract common patterns into base service classes
- Improve error handling consistency
- Add better logging throughout services
- Implement caching where appropriate

#### Step 3: Repository Pattern Improvements  
- Enhance repository interfaces with common query patterns
- Add specification pattern for complex queries
- Improve transaction handling
- Better handling of entity relationships

### Phase 4: Validation and Error Handling 🔄 FUTURE

#### Step 1: Centralized Validation
- Create validation attributes for common scenarios
- Implement fluent validation for complex models
- Standardize validation error responses
- Add client-side friendly validation messages

#### Step 2: Exception Handling Middleware
- Create global exception handling middleware
- Map specific exceptions to appropriate HTTP responses
- Add structured logging for exceptions
- Implement retry policies for transient failures

### Phase 5: Performance and Caching 🔄 FUTURE

#### Step 1: Caching Strategy
- Implement response caching for read-heavy endpoints
- Add distributed caching for frequently accessed data
- Create cache invalidation strategies
- Add cache warming for critical data

#### Step 2: Performance Optimizations
- Optimize database queries
- Implement pagination consistently
- Add response compression
- Optimize entity loading patterns

## 🔧 Implementation Guidelines

### Code Standards

**Controller Patterns:**
```csharp
[ApiController]
public class VenueManagementController : BaseApiController
{
    private readonly IVenueService _venueService;
    private readonly IPermissionService _permissionService;
    
    public VenueManagementController(
        IVenueService venueService,
        IPermissionService permissionService,
        ILogger<VenueManagementController> logger)
        : base(logger)
    {
        _venueService = venueService;
        _permissionService = permissionService;
    }
    
    [HttpGet(ApiRoutes.Venues.MyVenues)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VenueSummary>>>> GetMyVenues(
        CancellationToken cancellationToken = default)
    {
        LogActionStart(nameof(GetMyVenues));
        
        var (userSub, authError) = GetUserSubWithValidation();
        if (authError != null) return authError;
        
        try
        {
            var venueIds = await PermissionValidationHelper.GetAccessibleVenueIdsAsync(
                _permissionService, User, userSub!, cancellationToken);
            
            var result = await _venueService.GetVenuesByIdsAsync(venueIds, cancellationToken);
            
            LogActionComplete(nameof(GetMyVenues), result.Success);
            return HandleServiceResponse(result);
        }
        catch (Exception ex)
        {
            LogError(ex, nameof(GetMyVenues));
            return CreateErrorResponse(500, "An error occurred while retrieving venues");
        }
    }
}
```

**Service Patterns:**
```csharp
public class VenueService : IVenueService
{
    private readonly IVenueRepository _repository;
    private readonly ILogger<VenueService> _logger;
    
    public async Task<ApiResponse<IEnumerable<VenueSummary>>> GetVenuesByIdsAsync(
        IEnumerable<long> venueIds,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var venues = await _repository.GetVenuesByIdsAsync(venueIds, cancellationToken);
            var summaries = venues.Select(MapToVenueSummary);
            
            return ApiResponse<IEnumerable<VenueSummary>>.SuccessResult(summaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving venues by IDs: {VenueIds}", string.Join(", ", venueIds));
            return ApiResponse<IEnumerable<VenueSummary>>.ErrorResult("Failed to retrieve venues");
        }
    }
}
```

### Naming Conventions
- Controllers: `{Domain}Controller` (e.g., VenueManagementController)
- Services: `{Domain}Service` (e.g., VenueService)
- Repositories: `{Entity}Repository` (e.g., VenueRepository)
- Models: `{Purpose}{Type}` (e.g., CreateVenueRequest, VenueResponse)
- Constants: `{Domain}Constants` (e.g., DatabaseConstants)
- Helpers: `{Purpose}Helper` (e.g., PermissionValidationHelper)

### Error Handling Patterns
1. Use try-catch in controller actions
2. Log errors with context
3. Return standardized error responses
4. Map service layer errors appropriately
5. Include correlation IDs for tracking

### Logging Patterns
1. Log action start/completion
2. Include user context when available
3. Log parameters for debugging
4. Use structured logging
5. Include correlation IDs

## 📝 Checklist for Each Refactored Component

### Controller Checklist
- [ ] Inherits from BaseApiController
- [ ] Uses individual route attributes (no class-level routes)
- [ ] Implements proper authorization checks
- [ ] Uses helper classes for common patterns
- [ ] Has consistent error handling
- [ ] Includes proper logging
- [ ] Validates input using helpers
- [ ] Returns standardized responses
- [ ] Includes XML documentation
- [ ] Has appropriate unit tests

### Service Checklist  
- [ ] Implements service interface
- [ ] Returns ApiResponse<T> consistently
- [ ] Includes cancellation token support
- [ ] Has proper error handling
- [ ] Includes logging
- [ ] Uses repository pattern correctly
- [ ] Handles transactions appropriately
- [ ] Has mapping methods
- [ ] Includes validation logic
- [ ] Has appropriate unit tests

### Repository Checklist
- [ ] Implements repository interface
- [ ] Extends BaseRepository where appropriate
- [ ] Includes proper entity configurations
- [ ] Has optimized queries
- [ ] Supports async operations
- [ ] Includes cancellation token support
- [ ] Has proper error handling
- [ ] Includes integration tests

## 🚀 Next Steps

1. **Review this plan** with the development team
2. **Start with Phase 2** - Create the new domain controllers
3. **Move endpoints incrementally** - One controller at a time
4. **Test thoroughly** after each migration
5. **Update documentation** as you progress
6. **Remove BackofficeController** once all endpoints are migrated

## 📖 Additional Resources

- [SOLID Principles Guide](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles)
- [ASP.NET Core Controller Best Practices](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [Repository Pattern in .NET](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [API Design Guidelines](https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-design)
