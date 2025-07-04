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

public class SpecialsControllerTests
{
    private readonly Mock<ISpecialService> _mockSpecialService;
    private readonly Mock<ILogger<SpecialsController>> _mockLogger;
    private readonly SpecialsController _controller;

    public SpecialsControllerTests()
    {
        _mockSpecialService = new Mock<ISpecialService>();
        _mockLogger = new Mock<ILogger<SpecialsController>>();
        
        _controller = new SpecialsController(
            _mockSpecialService.Object,
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
    public async Task GetSpecials_ReturnsSpecials()
    {
        // Arrange
        var mockSpecials = new List<Domain.Entities.SpecialSummary>
        {
            new() { Id = 1, Title = "Test Special 1", VenueName = "Test Venue" },
            new() { Id = 2, Title = "Test Special 2", VenueName = "Test Venue" }
        };

        _mockSpecialService
            .Setup(x => x.GetSpecialsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSpecials);

        // Act
        var result = await _controller.GetSpecials();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.SpecialSummary>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact]
    public async Task GetSpecial_ValidId_ReturnsSpecial()
    {
        // Arrange
        var specialId = 1;
        var mockSpecial = new Domain.Entities.Special
        {
            Id = specialId,
            Title = "Test Special",
            Description = "Test Description",
            IsActive = true
        };

        _mockSpecialService
            .Setup(x => x.GetSpecialByIdAsync(specialId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSpecial);

        // Act
        var result = await _controller.GetSpecial(specialId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.Special>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(specialId, response.Data.Id);
        Assert.Equal("Test Special", response.Data.Title);
    }

    [Fact]
    public async Task GetSpecial_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var specialId = 999;

        _mockSpecialService
            .Setup(x => x.GetSpecialByIdAsync(specialId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Special?)null);

        // Act
        var result = await _controller.GetSpecial(specialId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.Special>>(notFoundResult.Value);
        
        Assert.False(response.Success);
        Assert.Equal("Special not found", response.ErrorMessage);
    }

    [Fact]
    public async Task GetActiveSpecials_ReturnsActiveSpecials()
    {
        // Arrange
        var mockSpecials = new List<Domain.Entities.SpecialSummary>
        {
            new() { Id = 1, Title = "Active Special", VenueName = "Test Venue", IsActive = true }
        };

        _mockSpecialService
            .Setup(x => x.GetActiveSpecialsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSpecials);

        // Act
        var result = await _controller.GetActiveSpecials();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.SpecialSummary>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.True(response.Data.First().IsActive);
    }

    [Fact]
    public async Task GetSpecialCategories_ReturnsCategories()
    {
        // Arrange
        var mockCategories = new List<Domain.Entities.SpecialCategory>
        {
            new() { Id = 1, Name = "Food", Description = "Food specials" },
            new() { Id = 2, Name = "Drink", Description = "Drink specials" }
        };

        _mockSpecialService
            .Setup(x => x.GetSpecialCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCategories);

        // Act
        var result = await _controller.GetSpecialCategories();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.SpecialCategory>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact]
    public async Task CreateSpecial_ValidRequest_ReturnsCreatedSpecial()
    {
        // Arrange
        var request = new Domain.Models.CreateSpecialRequest
        {
            Title = "New Special",
            Description = "A great special",
            VenueId = 1,
            CategoryId = 1
        };

        var mockSpecial = new Domain.Entities.Special
        {
            Id = 1,
            Title = request.Title,
            Description = request.Description,
            VenueId = request.VenueId,
            CategoryId = request.CategoryId,
            IsActive = true
        };

        _mockSpecialService
            .Setup(x => x.CreateSpecialAsync(It.IsAny<string>(), It.IsAny<Domain.Models.CreateSpecialRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSpecial);

        // Act
        var result = await _controller.CreateSpecial(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.Special>>(createdResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(request.Title, response.Data.Title);
    }

    [Fact]
    public async Task UpdateSpecial_ValidRequest_ReturnsUpdatedSpecial()
    {
        // Arrange
        var specialId = 1;
        var request = new Domain.Models.UpdateSpecialRequest
        {
            Title = "Updated Special",
            Description = "Updated description"
        };

        var mockSpecial = new Domain.Entities.Special
        {
            Id = specialId,
            Title = request.Title,
            Description = request.Description,
            IsActive = true
        };

        _mockSpecialService
            .Setup(x => x.UpdateSpecialAsync(It.IsAny<string>(), specialId, It.IsAny<Domain.Models.UpdateSpecialRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSpecial);

        // Act
        var result = await _controller.UpdateSpecial(specialId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.Special>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(request.Title, response.Data.Title);
    }

    [Fact]
    public async Task DeleteSpecial_ValidId_ReturnsSuccess()
    {
        // Arrange
        var specialId = 1;

        _mockSpecialService
            .Setup(x => x.DeleteSpecialAsync(It.IsAny<string>(), specialId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteSpecial(specialId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.True(response.Data);
    }

    [Fact]
    public async Task DeleteSpecial_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var specialId = 999;

        _mockSpecialService
            .Setup(x => x.DeleteSpecialAsync(It.IsAny<string>(), specialId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteSpecial(specialId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        
        Assert.False(response.Success);
        Assert.Equal("Special not found or no permission to delete", response.ErrorMessage);
    }

    [Fact]
    public async Task GetSpecialsByVenue_ValidVenueId_ReturnsSpecials()
    {
        // Arrange
        var venueId = 1;
        var mockSpecials = new List<Domain.Entities.SpecialSummary>
        {
            new() { Id = 1, Title = "Venue Special", VenueId = venueId, VenueName = "Test Venue" }
        };

        _mockSpecialService
            .Setup(x => x.GetSpecialsByVenueAsync(venueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSpecials);

        // Act
        var result = await _controller.GetSpecialsByVenue(venueId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.SpecialSummary>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal(venueId, response.Data.First().VenueId);
    }
}
