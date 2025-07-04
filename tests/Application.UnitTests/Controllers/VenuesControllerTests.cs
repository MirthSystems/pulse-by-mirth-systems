using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Services.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Application.UnitTests.Controllers;

public class VenuesControllerTests
{
    private readonly Mock<IVenueService> _mockVenueService;
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly Mock<IAuthorizationService> _mockAuthorizationService;
    private readonly Mock<ILogger<VenuesController>> _mockLogger;
    private readonly VenuesController _controller;

    public VenuesControllerTests()
    {
        _mockVenueService = new Mock<IVenueService>();
        _mockPermissionService = new Mock<IPermissionService>();
        _mockAuthorizationService = new Mock<IAuthorizationService>();
        _mockLogger = new Mock<ILogger<VenuesController>>();
        
        _controller = new VenuesController(
            _mockVenueService.Object,
            _mockPermissionService.Object,
            _mockAuthorizationService.Object,
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
    public async Task GetVenues_ReturnsVenues()
    {
        // Arrange
        var mockVenues = new List<Domain.Entities.VenueSummary>
        {
            new() { Id = 1, Name = "Test Venue 1", City = "Test City" },
            new() { Id = 2, Name = "Test Venue 2", City = "Test City" }
        };

        _mockVenueService
            .Setup(x => x.GetVenuesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockVenues);

        // Act
        var result = await _controller.GetVenues();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.VenueSummary>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact]
    public async Task GetVenue_ValidId_ReturnsVenue()
    {
        // Arrange
        var venueId = 1;
        var mockVenue = new Domain.Entities.Venue
        {
            Id = venueId,
            Name = "Test Venue",
            City = "Test City",
            IsActive = true
        };

        _mockVenueService
            .Setup(x => x.GetVenueByIdAsync(venueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockVenue);

        // Act
        var result = await _controller.GetVenue(venueId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.Venue>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(venueId, response.Data.Id);
        Assert.Equal("Test Venue", response.Data.Name);
    }

    [Fact]
    public async Task GetVenue_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var venueId = 999;

        _mockVenueService
            .Setup(x => x.GetVenueByIdAsync(venueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Venue?)null);

        // Act
        var result = await _controller.GetVenue(venueId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.Venue>>(notFoundResult.Value);
        
        Assert.False(response.Success);
        Assert.Equal("Venue not found", response.ErrorMessage);
    }

    [Fact]
    public async Task GetActiveVenues_ReturnsActiveVenues()
    {
        // Arrange
        var mockVenues = new List<Domain.Entities.VenueSummary>
        {
            new() { Id = 1, Name = "Active Venue 1", City = "Test City", IsActive = true }
        };

        _mockVenueService
            .Setup(x => x.GetActiveVenuesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockVenues);

        // Act
        var result = await _controller.GetActiveVenues();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.VenueSummary>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.True(response.Data.First().IsActive);
    }

    [Fact]
    public async Task GetVenueCategories_ReturnsCategories()
    {
        // Arrange
        var mockCategories = new List<Domain.Entities.VenueCategory>
        {
            new() { Id = 1, Name = "Restaurant", Description = "Food venue" },
            new() { Id = 2, Name = "Bar", Description = "Drink venue" }
        };

        _mockVenueService
            .Setup(x => x.GetVenueCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCategories);

        // Act
        var result = await _controller.GetVenueCategories();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.VenueCategory>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact]
    public async Task CreateVenue_ValidRequest_ReturnsCreatedVenue()
    {
        // Arrange
        var request = new Domain.Models.CreateVenueRequest
        {
            Name = "New Venue",
            Description = "A great venue",
            Address = "123 Test St",
            City = "Test City",
            CategoryId = 1
        };

        var mockVenue = new Domain.Entities.Venue
        {
            Id = 1,
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            CategoryId = request.CategoryId,
            IsActive = true
        };

        _mockVenueService
            .Setup(x => x.CreateVenueAsync(It.IsAny<string>(), It.IsAny<Domain.Models.CreateVenueRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockVenue);

        // Act
        var result = await _controller.CreateVenue(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.Venue>>(createdResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(request.Name, response.Data.Name);
    }

    [Fact]
    public async Task UpdateVenue_ValidRequest_ReturnsUpdatedVenue()
    {
        // Arrange
        var venueId = 1;
        var request = new Domain.Models.UpdateVenueRequest
        {
            Name = "Updated Venue",
            Description = "Updated description"
        };

        var mockVenue = new Domain.Entities.Venue
        {
            Id = venueId,
            Name = request.Name,
            Description = request.Description,
            IsActive = true
        };

        _mockVenueService
            .Setup(x => x.UpdateVenueAsync(It.IsAny<string>(), venueId, It.IsAny<Domain.Models.UpdateVenueRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockVenue);

        // Act
        var result = await _controller.UpdateVenue(venueId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.Venue>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(request.Name, response.Data.Name);
    }

    [Fact]
    public async Task DeleteVenue_ValidId_ReturnsSuccess()
    {
        // Arrange
        var venueId = 1;

        _mockVenueService
            .Setup(x => x.DeleteVenueAsync(It.IsAny<string>(), venueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteVenue(venueId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.True(response.Data);
    }

    [Fact]
    public async Task DeleteVenue_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var venueId = 999;

        _mockVenueService
            .Setup(x => x.DeleteVenueAsync(It.IsAny<string>(), venueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteVenue(venueId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        
        Assert.False(response.Success);
        Assert.Equal("Venue not found or no permission to delete", response.ErrorMessage);
    }
}
