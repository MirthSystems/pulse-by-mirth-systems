# 🗓️ Backend Refactoring Step-by-Step Execution Plan

## Current Status Assessment

### ✅ Completed
- Infrastructure constants and helpers created
- VenueManagementController started (with compilation issues)
- BACKEND_REFACTORING_INSTRUCTIONS.md exists
- Basic project structure analyzed

### ⚠️ Issues to Fix Immediately
- VenueManagementController has compilation errors
- Helper classes return ActionResult types but are in Common layer
- BaseApiController is empty
- Missing proper using directives

## 📋 Detailed Execution Steps

### Step 1: Fix Infrastructure Issues (Priority 1)

#### 1.1 Fix Helper Classes Return Types
**Issue**: Helper classes in `Application.Common.Utilities` are returning `ActionResult` types but don't have access to MVC types.

**Solution**: 
- Move helper methods that return ActionResult to controller base class or create wrapper types
- Keep utility methods in Common layer but return basic types
- Create controller-specific helpers in API layer

**Files to Update**:
- `src/Application/Common/Utilities/PermissionValidationHelper.cs`
- `src/Application/Common/Utilities/ControllerBaseHelper.cs`

#### 1.2 Create Proper BaseApiController
**Location**: `src/Application.Services.API/Controllers/Base/BaseApiController.cs`

**Content**:
- Common authorization patterns
- Logging helpers
- Error handling methods
- User context extraction

#### 1.3 Fix VenueManagementController
**Issues**:
- Missing using directives
- Type inference issues with helper methods
- ActionResult compilation errors

### Step 2: Complete VenueManagementController

#### 2.1 Fix Compilation Errors
- Add proper using statements
- Fix helper method calls
- Ensure proper return types

#### 2.2 Test VenueManagementController
- Verify all endpoints compile
- Test basic functionality
- Ensure proper routing

### Step 3: Create VenuePermissionController

#### 3.1 Analyze BackofficeController Permission Endpoints
Endpoints to migrate:
- `GET /api/backoffice/my-permissions`
- `GET /api/backoffice/venues/{venueId}/permissions`  
- `PUT /api/backoffice/permissions/{permissionId}`
- `DELETE /api/backoffice/permissions/{permissionId}`

#### 3.2 Create Controller Structure
```csharp
namespace Application.Services.API.Controllers;

[ApiController]
[Authorize]
public class VenuePermissionController : ControllerBase
{
    // Dependencies
    // Endpoints
}
```

#### 3.3 Implement Endpoints
- Follow established patterns
- Use ApiRoutes constants
- Implement proper error handling

### Step 4: Create VenueInvitationController

#### 4.1 Analyze BackofficeController Invitation Endpoints
Endpoints to migrate:
- `GET /api/backoffice/my-invitations`
- `GET /api/backoffice/venues/{venueId}/invitations`
- `POST /api/backoffice/invitations`
- `POST /api/backoffice/invitations/{invitationId}/accept`
- `POST /api/backoffice/invitations/{invitationId}/decline`
- `DELETE /api/backoffice/invitations/{invitationId}`

#### 4.2 Create Controller Structure
#### 4.3 Implement Endpoints

### Step 5: Create SpecialManagementController

#### 5.1 Analyze BackofficeController Special Endpoints
Endpoints to migrate:
- `GET /api/backoffice/specials`
- `POST /api/backoffice/venues/{venueId}/specials`
- `PUT /api/backoffice/specials/{id}`
- `DELETE /api/backoffice/specials/{id}`

#### 5.2 Create Controller Structure
#### 5.3 Implement Endpoints

### Step 6: Create UserManagementController

#### 6.1 Analyze BackofficeController User Endpoints
Endpoints to migrate:
- `POST /api/backoffice/sync-user`
- `GET /api/backoffice/permission-types`

#### 6.2 Create Controller Structure
#### 6.3 Implement Endpoints

### Step 7: Refactor Existing Controllers

#### 7.1 Update VenuesController
- Remove class-level `[Route]` attribute
- Add individual route attributes using ApiRoutes constants
- Implement consistent error handling patterns
- Use helper classes where applicable

#### 7.2 Update SpecialsController
- Same changes as VenuesController

#### 7.3 Update LocationController
- Review for consistency
- Add proper error handling if missing

### Step 8: Update Route Constants

#### 8.1 Review ApiRoutes.cs
- Ensure all new routes are defined
- Add any missing route constants
- Verify consistency

#### 8.2 Update Route Mappings
Example new routes needed:
```csharp
public static class Venues
{
    public const string MyVenues = "/api/venues/my";
    public const string Create = "/api/venues";
    public const string Update = "/api/venues/{id:long}";
    public const string Delete = "/api/venues/{id:long}";
    
    public static class Permissions
    {
        public const string My = "/api/venues/permissions/my";
        public const string ForVenue = "/api/venues/{venueId:long}/permissions";
        public const string Update = "/api/venues/permissions/{permissionId:long}";
        public const string Delete = "/api/venues/permissions/{permissionId:long}";
    }
    
    public static class Invitations
    {
        public const string My = "/api/venues/invitations/my";
        public const string ForVenue = "/api/venues/{venueId:long}/invitations";
        public const string Create = "/api/venues/invitations";
        public const string Accept = "/api/venues/invitations/{invitationId:long}/accept";
        public const string Decline = "/api/venues/invitations/{invitationId:long}/decline";
        public const string Cancel = "/api/venues/invitations/{invitationId:long}";
    }
}

public static class Specials
{
    public const string My = "/api/specials/my";
    public const string Create = "/api/specials";
    public const string Update = "/api/specials/{id:long}";
    public const string Delete = "/api/specials/{id:long}";
}

public static class Users
{
    public const string Sync = "/api/users/sync";
    public const string PermissionTypes = "/api/users/permission-types";
}
```

### Step 9: Remove BackofficeController

#### 9.1 Verify All Endpoints Migrated
- Create checklist of all BackofficeController endpoints
- Verify each has been moved to appropriate controller
- Test all migrated endpoints

#### 9.2 Update Any References
- Search for any imports of BackofficeController
- Update any documentation
- Remove from dependency injection if needed

#### 9.3 Delete BackofficeController
- Remove the file
- Verify clean compilation

### Step 10: Testing and Validation

#### 10.1 Compilation Testing
- Ensure entire solution compiles
- Fix any remaining compilation errors

#### 10.2 Endpoint Testing
- Test all migrated endpoints
- Verify routing works correctly
- Check authorization still functions

#### 10.3 Integration Testing
- Run existing integration tests
- Add new tests for refactored controllers
- Verify no regressions

### Step 11: Documentation and Cleanup

#### 11.1 Update API Documentation
- Update OpenAPI/Swagger documentation
- Verify all routes are documented correctly

#### 11.2 Code Cleanup
- Remove any unused using statements
- Verify consistent formatting
- Run code analysis tools

#### 11.3 Update README/Documentation
- Update any development documentation
- Add notes about new controller structure

## 🔍 Quality Checkpoints

### After Each Controller Creation:
- [ ] Compiles without errors
- [ ] All endpoints have individual route attributes
- [ ] No class-level [Route] attributes
- [ ] Proper dependency injection
- [ ] Consistent error handling
- [ ] Proper logging
- [ ] Authorization checks in place
- [ ] XML documentation comments

### After Completion:
- [ ] All BackofficeController endpoints migrated
- [ ] BackofficeController file removed
- [ ] All tests passing
- [ ] No compilation errors or warnings
- [ ] Consistent code patterns across controllers
- [ ] Updated route constants
- [ ] Documentation updated

## ⏱️ Estimated Timeline

- **Step 1 (Infrastructure)**: 2-3 hours
- **Step 2 (Fix VenueManagementController)**: 1 hour
- **Steps 3-6 (New Controllers)**: 4-6 hours (1-1.5 hours each)
- **Step 7 (Refactor Existing)**: 2-3 hours
- **Steps 8-11 (Cleanup/Testing)**: 2-3 hours

**Total Estimated Time**: 11-16 hours

## 🚨 Risk Mitigation

### Backup Strategy
- Create feature branch before starting
- Commit after each major step
- Keep BackofficeController until all endpoints verified

### Testing Strategy
- Test each controller as it's created
- Maintain existing integration tests
- Add controller-specific unit tests

### Rollback Plan
- If issues arise, can revert to specific commits
- BackofficeController remains as backup until Step 9
