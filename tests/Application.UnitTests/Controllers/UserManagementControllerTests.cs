using Application.Common.Constants;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Application.Services.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Application.UnitTests.Controllers;

public class UserManagementControllerTests
{
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<UserManagementController>> _mockLogger;
    private readonly UserManagementController _controller;

    public UserManagementControllerTests()
    {
        _mockPermissionService = new Mock<IPermissionService>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<UserManagementController>>();
        
        _controller = new UserManagementController(
            _mockPermissionService.Object,
            _mockConfiguration.Object,
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
    public async Task SyncUser_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new UserInfo
        {
            Sub = "test-user-123",
            Email = "test@example.com",
            Name = "Test User"
        };

        var mockUser = new Domain.Entities.User
        {
            Id = 1,
            Sub = request.Sub,
            Email = request.Email,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            LastLoginAt = DateTimeOffset.UtcNow
        };

        _mockPermissionService
            .Setup(x => x.EnsureUserExistsAsync(request.Sub, request.Email, request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockPermissionService
            .Setup(x => x.GetUserBySubAsync(request.Sub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockUser);

        // Act
        var result = await _controller.SyncUser(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserInfoResponse>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(request.Sub, response.Data.Sub);
        Assert.Equal(request.Email, response.Data.Email);
        Assert.Equal(request.Name, response.Data.Name);
    }

    [Fact]
    public async Task SyncUser_UserNotFoundAfterSync_ReturnsBadRequest()
    {
        // Arrange
        var request = new UserInfo
        {
            Sub = "test-user-123",
            Email = "test@example.com",
            Name = "Test User"
        };

        _mockPermissionService
            .Setup(x => x.EnsureUserExistsAsync(request.Sub, request.Email, request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockPermissionService
            .Setup(x => x.GetUserBySubAsync(request.Sub, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act
        var result = await _controller.SyncUser(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserInfoResponse>>(badRequestResult.Value);
        
        Assert.False(response.Success);
        Assert.Equal("Failed to retrieve user after sync", response.ErrorMessage);
    }

    [Fact]
    public async Task GetProfile_ValidUser_ReturnsUserProfile()
    {
        // Arrange
        var userSub = "test-user-123";
        var mockUser = new Domain.Entities.User
        {
            Id = 1,
            Sub = userSub,
            Email = "test@example.com",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            LastLoginAt = DateTimeOffset.UtcNow
        };

        _mockPermissionService
            .Setup(x => x.GetUserBySubAsync(userSub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockUser);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserInfoResponse>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(userSub, response.Data.Sub);
        Assert.Equal(mockUser.Email, response.Data.Email);
    }

    [Fact]
    public async Task GetProfile_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userSub = "test-user-123";
        
        _mockPermissionService
            .Setup(x => x.GetUserBySubAsync(userSub, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserInfoResponse>>(notFoundResult.Value);
        
        Assert.False(response.Success);
        Assert.Equal("User not found", response.ErrorMessage);
    }

    [Fact]
    public async Task SyncUser_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var request = new UserInfo
        {
            Sub = "test-user-123",
            Email = "test@example.com",
            Name = "Test User"
        };

        _mockPermissionService
            .Setup(x => x.EnsureUserExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.SyncUser(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}
