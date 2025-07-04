using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Services.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.UnitTests.Controllers;

public class LocationControllerTests
{
    private readonly Mock<ILocationService> _mockLocationService;
    private readonly Mock<ILogger<LocationController>> _mockLogger;
    private readonly LocationController _controller;

    public LocationControllerTests()
    {
        _mockLocationService = new Mock<ILocationService>();
        _mockLogger = new Mock<ILogger<LocationController>>();
        
        _controller = new LocationController(
            _mockLocationService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task SearchLocation_ValidQuery_ReturnsResults()
    {
        // Arrange
        var query = "Seattle";
        var mockResults = new List<Domain.Entities.PointOfInterest>
        {
            new() { Name = "Seattle, WA", Latitude = 47.6062, Longitude = -122.3321 }
        };

        _mockLocationService
            .Setup(x => x.SearchLocationAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResults);

        // Act
        var result = await _controller.SearchLocation(query);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.PointOfInterest>>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal("Seattle, WA", response.Data.First().Name);
    }

    [Fact]
    public async Task SearchLocation_EmptyQuery_ReturnsBadRequest()
    {
        // Arrange
        var query = "";

        // Act
        var result = await _controller.SearchLocation(query);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Domain.Entities.PointOfInterest>>>(badRequestResult.Value);
        
        Assert.False(response.Success);
        Assert.Equal("Query parameter is required", response.ErrorMessage);
    }

    [Fact]
    public async Task GeocodeAddress_ValidRequest_ReturnsResult()
    {
        // Arrange
        var request = new Domain.Models.GeocodeRequest
        {
            Address = "123 Main St, Seattle, WA"
        };

        var mockResult = new Domain.Entities.GeocodeResult
        {
            Address = request.Address,
            Latitude = 47.6062,
            Longitude = -122.3321,
            Confidence = 0.95
        };

        _mockLocationService
            .Setup(x => x.GeocodeAddressAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult);

        // Act
        var result = await _controller.GeocodeAddress(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.GeocodeResult>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(request.Address, response.Data.Address);
        Assert.Equal(47.6062, response.Data.Latitude, 4);
        Assert.Equal(-122.3321, response.Data.Longitude, 4);
    }

    [Fact]
    public async Task ReverseGeocode_ValidRequest_ReturnsResult()
    {
        // Arrange
        var request = new Domain.Models.ReverseGeocodeRequest
        {
            Latitude = 47.6062,
            Longitude = -122.3321
        };

        var mockResult = new Domain.Entities.ReverseGeocodeResult
        {
            Address = "123 Main St, Seattle, WA 98101",
            City = "Seattle",
            State = "WA",
            PostalCode = "98101",
            Country = "USA"
        };

        _mockLocationService
            .Setup(x => x.ReverseGeocodeAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult);

        // Act
        var result = await _controller.ReverseGeocode(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.ReverseGeocodeResult>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("Seattle", response.Data.City);
        Assert.Equal("WA", response.Data.State);
    }

    [Fact]
    public async Task GetTimeZone_ValidCoordinates_ReturnsTimeZone()
    {
        // Arrange
        var latitude = 47.6062;
        var longitude = -122.3321;
        
        var mockTimeZone = new Domain.Entities.TimeZoneInfo
        {
            Id = "America/Los_Angeles",
            Name = "Pacific Standard Time",
            Abbreviation = "PST",
            OffsetFromUtc = TimeSpan.FromHours(-8)
        };

        _mockLocationService
            .Setup(x => x.GetTimeZoneAsync(latitude, longitude, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTimeZone);

        // Act
        var result = await _controller.GetTimeZone(latitude, longitude);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.TimeZoneInfo>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("America/Los_Angeles", response.Data.Id);
        Assert.Equal("Pacific Standard Time", response.Data.Name);
    }

    [Fact]
    public async Task ValidateAddress_ValidRequest_ReturnsValidation()
    {
        // Arrange
        var request = new Domain.Models.AddressValidationRequest
        {
            Address = "123 Main St",
            City = "Seattle",
            State = "WA",
            PostalCode = "98101"
        };

        var mockValidation = new Domain.Entities.AddressValidationResult
        {
            IsValid = true,
            FormattedAddress = "123 Main St, Seattle, WA 98101",
            Confidence = 0.95
        };

        _mockLocationService
            .Setup(x => x.ValidateAddressAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockValidation);

        // Act
        var result = await _controller.ValidateAddress(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.AddressValidationResult>>(okResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.True(response.Data.IsValid);
        Assert.Equal("123 Main St, Seattle, WA 98101", response.Data.FormattedAddress);
    }

    [Fact]
    public async Task GeocodeAddress_ServiceReturnsNull_ReturnsNotFound()
    {
        // Arrange
        var request = new Domain.Models.GeocodeRequest
        {
            Address = "Invalid Address"
        };

        _mockLocationService
            .Setup(x => x.GeocodeAddressAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.GeocodeResult?)null);

        // Act
        var result = await _controller.GeocodeAddress(request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Domain.Entities.GeocodeResult>>(notFoundResult.Value);
        
        Assert.False(response.Success);
        Assert.Equal("Address could not be geocoded", response.ErrorMessage);
    }

    [Fact]
    public async Task SearchLocation_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var query = "test";
        
        _mockLocationService
            .Setup(x => x.SearchLocationAsync(query, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.SearchLocation(query);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}
