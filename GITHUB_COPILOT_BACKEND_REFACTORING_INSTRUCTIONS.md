# 🤖 GitHub Copilot Backend Refactoring Instructions

## 📋 Context
You are GitHub Copilot assisting with refactoring the Pulse backend application to follow SOLID and DRY principles, improve controller organization, and enhance maintainability. The current state includes a monolithic BackofficeController that needs to be split into domain-specific controllers.

## 🎯 Primary Goals

### 1. SOLID Principles Implementation
- **Single Responsibility**: Each controller handles one domain area
- **Open/Closed**: Extensible without modification through interfaces
- **Liskov Substitution**: Proper inheritance hierarchies
- **Interface Segregation**: Focused, specific interfaces
- **Dependency Inversion**: Depend on abstractions, not concretions

### 2. DRY (Don't Repeat Yourself)
- Eliminate duplicate code through helpers and utilities
- Create reusable patterns for common operations
- Standardize error handling and validation
- Consistent logging and response patterns

### 3. Controller Organization
- Split BackofficeController into logical domain controllers
- Remove class-level route prefixes
- Ensure each endpoint has its own route
- Proper separation of concerns

## 🛠️ Available Infrastructure

### Constants
- `AppConstants.cs` - Application-wide constants
- `ApiRoutes.cs` - Route constants (USE THESE EXCLUSIVELY)
- `AuthorizationConstants.cs` - Authorization constants  
- `ErrorMessages.cs` - Error message constants
- `HttpStatusConstants.cs` - HTTP status codes
- `DatabaseConstants.cs` - Database-related constants

### Helper Classes
- `ResponseBuilder.cs` - API response construction
- `UserContextHelper.cs` - User context extraction
- `ControllerAuthorizationHelper.cs` - Authorization patterns
- `ControllerBaseHelper.cs` - Common controller operations
- `SearchParameterHelper.cs` - Search parameter validation
- `PermissionValidationHelper.cs` - Permission checking

## 📊 Current State Analysis

### BackofficeController Endpoints to Migrate

#### Venue Management (Move to VenueManagementController)
- `GET /api/backoffice/venues` → `GET /api/venues/my`
- `POST /api/backoffice/venues` → `POST /api/venues`
- `PUT /api/backoffice/venues/{id}` → `PUT /api/venues/{id}`
- `DELETE /api/backoffice/venues/{id}` → `DELETE /api/venues/{id}`

#### Venue Permissions (Move to VenuePermissionController)
- `GET /api/backoffice/my-permissions` → `GET /api/venues/permissions/my`
- `GET /api/backoffice/venues/{venueId}/permissions` → `GET /api/venues/{venueId}/permissions`
- `PUT /api/backoffice/permissions/{permissionId}` → `PUT /api/venues/permissions/{permissionId}`
- `DELETE /api/backoffice/permissions/{permissionId}` → `DELETE /api/venues/permissions/{permissionId}`

#### Venue Invitations (Move to VenueInvitationController)
- `GET /api/backoffice/my-invitations` → `GET /api/venues/invitations/my`
- `GET /api/backoffice/venues/{venueId}/invitations` → `GET /api/venues/{venueId}/invitations`
- `POST /api/backoffice/invitations` → `POST /api/venues/invitations`
- `POST /api/backoffice/invitations/{invitationId}/accept` → `POST /api/venues/invitations/{invitationId}/accept`
- `POST /api/backoffice/invitations/{invitationId}/decline` → `POST /api/venues/invitations/{invitationId}/decline`
- `DELETE /api/backoffice/invitations/{invitationId}` → `DELETE /api/venues/invitations/{invitationId}`

#### Special Management (Move to SpecialManagementController)
- `GET /api/backoffice/specials` → `GET /api/specials/my`
- `POST /api/backoffice/venues/{venueId}/specials` → `POST /api/specials`
- `PUT /api/backoffice/specials/{id}` → `PUT /api/specials/{id}`
- `DELETE /api/backoffice/specials/{id}` → `DELETE /api/specials/{id}`

#### User Management (Move to UserManagementController)
- `POST /api/backoffice/sync-user` → `POST /api/users/sync`
- `GET /api/backoffice/permission-types` → `GET /api/users/permission-types`

## 🔧 Refactoring Instructions

### Phase 1: Fix Current Issues ⚠️ PRIORITY
1. **Fix VenueManagementController compilation errors**
   - Add missing ActionResult imports
   - Fix helper method return type issues
   - Ensure proper type inference for deconstruction

2. **Create proper BaseApiController**
   - Common logging patterns
   - Error handling
   - Authorization helpers

### Phase 2: Controller Creation Pattern

#### When creating new controllers, follow this pattern:

```csharp
using Application.Common.Constants;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Common.Utilities;
using Application.Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Application.Services.API.Controllers;

/// <summary>
/// [Controller Description]
/// </summary>
[ApiController]
[Authorize]
public class [ControllerName]Controller : ControllerBase
{
    private readonly [Required Services];
    private readonly ILogger<[ControllerName]Controller> _logger;

    public [ControllerName]Controller([Dependencies])
    {
        // Dependency assignment with null checks
    }

    // Endpoints using individual routes from ApiRoutes constants
}
```

#### Key Patterns to Follow:

1. **Route Decoration**
   ```csharp
   [HttpGet(ApiRoutes.Venues.MyVenues)]
   [HttpPost(ApiRoutes.Venues.Create)]
   [HttpPut(ApiRoutes.Venues.Update)]
   [HttpDelete(ApiRoutes.Venues.Delete)]
   ```

2. **Authorization Checking**
   ```csharp
   // Check backoffice access
   var authResult = await _authorizationService.AuthorizeAsync(User, null, new BackofficeAccessRequirement());
   if (!authResult.Succeeded)
   {
       return Forbid();
   }
   ```

3. **User Context Extraction**
   ```csharp
   var userSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
   if (string.IsNullOrEmpty(userSub))
   {
       return Unauthorized();
   }
   ```

4. **Error Handling Pattern**
   ```csharp
   try
   {
       // Operation logic
       var result = await _service.OperationAsync(parameters, cancellationToken);
       
       if (!result.Success)
       {
           return BadRequest(result);
       }
       
       return Ok(result);
   }
   catch (Exception ex)
   {
       _logger.LogError(ex, "Error message with context");
       return StatusCode(500, ApiResponse<T>.ErrorResult(ErrorMessages.InternalServerError));
   }
   ```

5. **Logging Pattern**
   ```csharp
   _logger.LogInformation("Operation started for user {UserSub}", userSub);
   _logger.LogError(ex, "Error during operation for user {UserSub}", userSub);
   ```

### Phase 3: Update Existing Controllers

#### VenuesController
- Remove class-level route attribute
- Add individual route attributes from ApiRoutes
- Implement consistent error handling
- Use helper classes

#### SpecialsController  
- Remove class-level route attribute
- Add individual route attributes from ApiRoutes
- Implement consistent error handling
- Use helper classes

#### LocationController
- Review and ensure consistency
- Add proper error handling
- Use helper classes where applicable

### Phase 4: Remove BackofficeController
- Ensure all endpoints are migrated
- Update any remaining references
- Remove the file

## 🚀 Step-by-Step Execution Plan

### Step 1: Fix Immediate Issues
1. Resolve VenueManagementController compilation errors
2. Create proper BaseApiController implementation
3. Fix helper class return types and imports

### Step 2: Create New Controllers (In Order)
1. **VenuePermissionController** - Venue permission management
2. **VenueInvitationController** - Venue invitation management  
3. **SpecialManagementController** - Special management operations
4. **UserManagementController** - User management operations

### Step 3: Refactor Existing Controllers
1. Update VenuesController to use new patterns
2. Update SpecialsController to use new patterns
3. Update LocationController to use new patterns

### Step 4: Cleanup and Testing
1. Remove BackofficeController
2. Update route constants if needed
3. Test all endpoints
4. Update documentation

## ⚡ Helper Usage Guidelines

### Always Use These Helpers:

1. **ApiRoutes Constants**
   ```csharp
   [HttpGet(ApiRoutes.Venues.GetById)]
   ```

2. **Error Messages**
   ```csharp
   return StatusCode(500, ApiResponse<T>.ErrorResult(ErrorMessages.InternalServerError));
   ```

3. **HTTP Status Constants**
   ```csharp
   return StatusCode(HttpStatusConstants.InternalServerError, response);
   ```

4. **Response Builder**
   ```csharp
   var response = ApiResponse<T>.SuccessResult(data);
   ```

5. **User Context Helper**
   ```csharp
   var userContext = UserContextHelper.GetUserContext(User);
   ```

## 🔍 Validation Checklist

### For Each New Controller:
- [ ] Inherits from ControllerBase (or BaseApiController when available)
- [ ] Uses ApiRoutes constants for all route attributes
- [ ] No class-level [Route] attribute
- [ ] Proper dependency injection with null checks
- [ ] Consistent error handling with try/catch
- [ ] Proper logging using ILogger
- [ ] Authorization checks using AuthorizationService
- [ ] Returns appropriate HTTP status codes
- [ ] Uses ApiResponse<T> wrapper for responses
- [ ] Includes XML documentation comments

### For Each Endpoint:
- [ ] Individual [Http*] route attribute
- [ ] Proper parameter validation
- [ ] Authorization checks where needed
- [ ] Error handling and logging
- [ ] Consistent response format
- [ ] CancellationToken support
- [ ] Appropriate HTTP status codes

## 🎯 Success Criteria
1. All BackofficeController endpoints migrated to appropriate domain controllers
2. No class-level route prefixes
3. Consistent use of helper classes and constants
4. Proper error handling and logging
5. All controllers follow the same pattern
6. No compilation errors
7. All endpoints accessible at new routes
8. Maintainable and extensible code structure

## 📝 Notes for Copilot
- Always reference existing constants rather than hardcoding values
- Maintain consistent patterns across all controllers
- Prioritize readability and maintainability
- Use the established helper classes to reduce code duplication
- Follow ASP.NET Core best practices
- Ensure proper async/await patterns
- Include comprehensive error handling
