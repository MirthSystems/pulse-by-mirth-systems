using Application.Common.Models;
using Application.Services.API.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Application.UnitTests.Controllers;

public class BaseApiControllerTests
{
    private class TestController : BaseApiController
    {
        public TestController(ILogger<TestController> logger) : base(logger) { }

        // Expose protected methods for testing
        public new ActionResult<ApiResponse<T>> ValidateModelState<T>() => base.ValidateModelState<T>();
        public new ObjectResult InternalServerError<T>() => base.InternalServerError<T>();
        public new void LogActionStart(string actionName, object? request = null) => base.LogActionStart(actionName, request);
        public new void LogActionComplete(string actionName, bool success) => base.LogActionComplete(actionName, success);
        public new void LogError(Exception ex, string actionName, object? request = null) => base.LogError(ex, actionName, request);
    }

    private readonly Mock<ILogger<TestController>> _mockLogger;
    private readonly TestController _controller;

    public BaseApiControllerTests()
    {
        _mockLogger = new Mock<ILogger<TestController>>();
        _controller = new TestController(_mockLogger.Object);

        // Setup HttpContext
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
    public void ValidateModelState_ValidModel_ReturnsNull()
    {
        // Arrange - ModelState is valid by default

        // Act
        var result = _controller.ValidateModelState<string>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ValidateModelState_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("TestProperty", "Test error message");

        // Act
        var result = _controller.ValidateModelState<string>();

        // Assert
        Assert.NotNull(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
        
        Assert.False(response.Success);
        Assert.Contains("TestProperty: Test error message", response.ErrorMessage);
    }

    [Fact]
    public void InternalServerError_ReturnsCorrectStatusAndResponse()
    {
        // Act
        var result = _controller.InternalServerError<string>();

        // Assert
        Assert.Equal(500, result.StatusCode);
        var response = Assert.IsType<ApiResponse<string>>(result.Value);
        
        Assert.False(response.Success);
        Assert.Equal("An internal server error occurred", response.ErrorMessage);
        Assert.Null(response.Data);
    }

    [Fact]
    public void LogActionStart_LogsCorrectly()
    {
        // Arrange
        var actionName = "TestAction";
        var request = new { Property = "Value" };

        // Act
        _controller.LogActionStart(actionName, request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Starting {actionName}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogActionStart_WithoutRequest_LogsCorrectly()
    {
        // Arrange
        var actionName = "TestAction";

        // Act
        _controller.LogActionStart(actionName);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Starting {actionName}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogActionComplete_Success_LogsCorrectly()
    {
        // Arrange
        var actionName = "TestAction";

        // Act
        _controller.LogActionComplete(actionName, true);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Completed {actionName} successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogActionComplete_Failure_LogsCorrectly()
    {
        // Arrange
        var actionName = "TestAction";

        // Act
        _controller.LogActionComplete(actionName, false);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Completed {actionName} with failure")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogError_LogsErrorCorrectly()
    {
        // Arrange
        var actionName = "TestAction";
        var exception = new Exception("Test exception");
        var request = new { Property = "Value" };

        // Act
        _controller.LogError(exception, actionName, request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Error in {actionName}")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogError_WithoutRequest_LogsErrorCorrectly()
    {
        // Arrange
        var actionName = "TestAction";
        var exception = new Exception("Test exception");

        // Act
        _controller.LogError(exception, actionName);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Error in {actionName}")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void ValidateModelState_MultipleErrors_CombinesMessages()
    {
        // Arrange
        _controller.ModelState.AddModelError("Property1", "Error 1");
        _controller.ModelState.AddModelError("Property2", "Error 2");
        _controller.ModelState.AddModelError("Property1", "Error 3"); // Multiple errors for same property

        // Act
        var result = _controller.ValidateModelState<string>();

        // Assert
        Assert.NotNull(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
        
        Assert.False(response.Success);
        Assert.Contains("Property1: Error 1, Error 3", response.ErrorMessage);
        Assert.Contains("Property2: Error 2", response.ErrorMessage);
    }
}
