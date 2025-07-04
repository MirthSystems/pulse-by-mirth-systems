# Backend API Refactoring Plan

## GitHub Copilot Instructions for Backend Refactoring

### Pre-Code Analysis Steps

1. **Architectural Review**
   - Audit all controllers for single responsibility principle violations
   - Identify duplicated code patterns across controllers and services
   - Map out current dependency injection patterns
   - Review authorization and authentication patterns
   - Analyze current error handling approaches

2. **SOLID Principles Assessment**
   - **Single Responsibility**: Each controller should handle one domain
   - **Open/Closed**: Services should be extensible without modification
   - **Liskov Substitution**: Interfaces should be properly abstracted
   - **Interface Segregation**: Split large interfaces into focused ones
   - **Dependency Inversion**: Controllers should depend on abstractions

3. **DRY Violations Identification**
   - Common validation logic repetition
   - Duplicate error handling patterns
   - Repeated authorization checks
   - Common response mapping logic

### Refactoring Strategy

#### Phase 1: Infrastructure Foundation
1. **Create Constants Library**
   - API route patterns
   - Error messages
   - Authorization policies
   - HTTP status codes
   - Validation messages

2. **Create Utilities Library**
   - Response builders
   - Exception handlers
   - Validation helpers
   - Mapping utilities
   - Security helpers

3. **Create Result Pattern**
   - Replace ApiResponse<T> with more robust Result<T>
   - Include error details and status codes
   - Support multiple error types

#### Phase 2: Controller Restructuring
1. **Break up BackofficeController**
   - Move venue management to VenuesController with route attributes
   - Move special management to SpecialsController with route attributes
   - Create PermissionsController for user/permission management
   - Create InvitationsController for invitation management

2. **Remove Route Prefixes**
   - Replace [Route("api/venues")] with individual [Route] attributes per action
   - Enable better route organization and versioning

3. **Create Base Controller**
   - Common authorization logic
   - Standard error handling
   - User context extraction
   - Response formatting

#### Phase 3: Service Layer Enhancement
1. **Split Large Services**
   - Break VenueService into focused services
   - Create specialized command/query services
   - Implement CQRS pattern for complex operations

2. **Create Business Logic Layer**
   - Move domain logic out of controllers
   - Create use case handlers
   - Implement domain events

#### Phase 4: Cross-Cutting Concerns
1. **Implement Middleware**
   - Global exception handling
   - Request/response logging
   - Performance monitoring
   - Security headers

2. **Create Filters and Attributes**
   - Custom authorization attributes
   - Validation filters
   - Audit logging filters

### Current Issues Identified

#### BackofficeController Problems
- **1189 lines** - Violates Single Responsibility Principle
- **Mixed Concerns**: Venues, Specials, Permissions, Invitations all in one controller
- **Route Inconsistency**: Uses controller-level route prefix
- **Duplicate Authorization Logic**: Repeated user context extraction
- **Inconsistent Error Handling**: Mixed patterns across endpoints

#### VenuesController Issues
- **Missing Management Endpoints**: No CRUD operations
- **Route Prefix Problem**: Prevents flexible routing
- **Limited Error Handling**: Basic patterns only

#### Service Layer Issues
- **Fat Services**: Services doing too much
- **Tight Coupling**: Controllers tightly coupled to service implementations
- **Missing Abstractions**: Some business logic in controllers

#### Missing Infrastructure
- **No Constants**: Magic strings throughout codebase
- **No Utilities**: Repeated helper code
- **No Result Pattern**: Basic ApiResponse not flexible enough
- **No Middleware**: Manual error handling in each controller

### Implementation Priority

1. **High Priority** (SOLID/DRY violations)
   - Create Constants and Utilities
   - Implement Result pattern
   - Break up BackofficeController
   - Remove route prefixes

2. **Medium Priority** (Architecture improvements)
   - Create base controller
   - Implement middleware
   - Add custom attributes
   - Split large services

3. **Low Priority** (Future enhancements)
   - Implement CQRS
   - Add domain events
   - Create use case handlers

### Success Metrics

- **Reduced Code Duplication**: <5% duplicate code blocks
- **Controller Size**: <200 lines per controller
- **Service Cohesion**: Single responsibility per service
- **Route Consistency**: All routes defined at action level
- **Error Handling**: Centralized and consistent
- **Test Coverage**: >80% for new components

### Next Steps

1. Create folder structure for Constants and Utilities
2. Implement Result pattern and common interfaces
3. Create base controller with common functionality
4. Break up BackofficeController methodically
5. Add route attributes to all actions
6. Implement global exception middleware
7. Add comprehensive logging
8. Create integration tests for refactored endpoints
