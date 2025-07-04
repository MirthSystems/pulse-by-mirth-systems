using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Services.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Application.UnitTests.Controllers;

public class VenuePermissionControllerTests
{
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly Mock<ILogger<VenuePermissionController>> _mockLogger;
    private readonly VenuePermissionController _controller;

    public VenuePermissionControllerTests()
    {
        _mockPermissionService = new Mock<IPermissionService>();
        _mockLogger = new Mock<ILogger<VenuePermissionController>>();
        
        _controller = new VenuePermissionController(
            _mockPermissionService.Object,
            _mockLogger.Object);

        // Setup HttpContext with a user
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("sub", "test-user-123"),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.Email, "test@example.com")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Fact]
    public async Task GetMyPermissions_ReturnsUserPermissions()
    {
        // Arrange
        var mockPermissions = new List<Domain.Entities.UserVenuePermission>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                VenueId = 1,
                VenueName = "Test Venue",
                PermissionType = Domain.Enums.VenuePermissionType.Manager,
                IsActive = true
            }
        };

        _mockPermissionService
            .Setup(x => x.GetUserPermissionsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockPermissions);

        // Act
        var result = await _controller.GetMyPermissions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.UserVenuePermission>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal("Test Venue", response.Data.First().VenueName);
    }

    [Fact]
    public async Task CreateInvitation_ValidRequest_ReturnsInvitation()
    {
        // Arrange
        var request = new Domain.Models.CreateInvitationRequest
        {
            VenueId = 1,
            Email = "invitee@example.com",
            PermissionType = Domain.Enums.VenuePermissionType.Staff
        };

        var mockInvitation = new Domain.Entities.VenueInvitationResponse
        {
            Id = 1,
            VenueId = request.VenueId,
            Email = request.Email,
            PermissionType = request.PermissionType,
            Status = Domain.Enums.InvitationStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _mockPermissionService
            .Setup(x => x.CreateInvitationAsync(It.IsAny<string>(), It.IsAny<Domain.Models.CreateInvitationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockInvitation);

        // Act
        var result = await _controller.CreateInvitation(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.VenueInvitationResponse>>(createdResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(request.Email, response.Data.Email);
        Assert.Equal(request.VenueId, response.Data.VenueId);
    }

    [Fact]
    public async Task GetMyInvitations_ReturnsInvitations()
    {
        // Arrange
        var email = "test@example.com";
        var mockInvitations = new List<Domain.Entities.VenueInvitationResponse>
        {
            new()
            {
                Id = 1,
                VenueId = 1,
                VenueName = "Test Venue",
                Email = email,
                PermissionType = Domain.Enums.VenuePermissionType.Staff,
                Status = Domain.Enums.InvitationStatus.Pending
            }
        };

        _mockPermissionService
            .Setup(x => x.GetUserInvitationsAsync(It.IsAny<string>(), email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockInvitations);

        // Act
        var result = await _controller.GetMyInvitations(email);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.VenueInvitationResponse>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal(email, response.Data.First().Email);
    }

    [Fact]
    public async Task GetPermissionTypes_ReturnsPermissionTypes()
    {
        // Arrange
        var mockTypes = new List<Domain.Entities.PermissionTypeResponse>
        {
            new() { Id = 1, Name = "Manager", Description = "Full management access" },
            new() { Id = 2, Name = "Staff", Description = "Basic staff access" }
        };

        _mockPermissionService
            .Setup(x => x.GetPermissionTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTypes);

        // Act
        var result = await _controller.GetPermissionTypes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.PermissionTypeResponse>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact]
    public async Task UpdatePermission_ValidRequest_ReturnsUpdatedPermission()
    {
        // Arrange
        var permissionId = 1;
        var request = new Domain.Models.UpdatePermissionRequest
        {
            PermissionType = Domain.Enums.VenuePermissionType.Manager
        };

        var mockPermission = new Domain.Entities.UserVenuePermission
        {
            Id = permissionId,
            PermissionType = request.PermissionType,
            IsActive = true
        };

        _mockPermissionService
            .Setup(x => x.UpdatePermissionAsync(It.IsAny<string>(), permissionId, It.IsAny<Domain.Models.UpdatePermissionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockPermission);

        // Act
        var result = await _controller.UpdatePermission(permissionId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.UserVenuePermission>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(request.PermissionType, response.Data.PermissionType);
    }

    [Fact]
    public async Task RevokePermission_ValidId_ReturnsSuccess()
    {
        // Arrange
        var permissionId = 1;

        _mockPermissionService
            .Setup(x => x.RevokePermissionAsync(It.IsAny<string>(), permissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.RevokePermission(permissionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.True(response.Data);
    }

    [Fact]
    public async Task RevokePermission_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var permissionId = 999;

        _mockPermissionService
            .Setup(x => x.RevokePermissionAsync(It.IsAny<string>(), permissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.RevokePermission(permissionId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        
        Assert.False(response.Success);
        Assert.Equal("Permission not found or no access to revoke", response.ErrorMessage);
    }

    [Fact]
    public async Task AcceptInvitation_ValidId_ReturnsSuccess()
    {
        // Arrange
        var invitationId = 1;

        _mockPermissionService
            .Setup(x => x.AcceptInvitationAsync(It.IsAny<string>(), invitationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.AcceptInvitation(invitationId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.True(response.Data);
    }

    [Fact]
    public async Task DeclineInvitation_ValidId_ReturnsSuccess()
    {
        // Arrange
        var invitationId = 1;

        _mockPermissionService
            .Setup(x => x.DeclineInvitationAsync(It.IsAny<string>(), invitationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeclineInvitation(invitationId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.True(response.Data);
    }

    [Fact]
    public async Task GetVenuePermissions_ValidVenueId_ReturnsPermissions()
    {
        // Arrange
        var venueId = 1;
        var mockPermissions = new List<Domain.Entities.UserVenuePermission>
        {
            new()
            {
                Id = 1,
                VenueId = venueId,
                VenueName = "Test Venue",
                UserEmail = "user@example.com",
                PermissionType = Domain.Enums.VenuePermissionType.Manager
            }
        };

        _mockPermissionService
            .Setup(x => x.GetVenuePermissionsAsync(It.IsAny<string>(), venueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockPermissions);

        // Act
        var result = await _controller.GetVenuePermissions(venueId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.UserVenuePermission>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal(venueId, response.Data.First().VenueId);
    }
}
