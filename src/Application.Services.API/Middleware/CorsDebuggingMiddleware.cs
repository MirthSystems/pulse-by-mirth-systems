using System.Diagnostics;

namespace Application.Services.API.Middleware;

/// <summary>
/// Middleware to log CORS-related information for debugging
/// </summary>
public class CorsDebuggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorsDebuggingMiddleware> _logger;

    public CorsDebuggingMiddleware(RequestDelegate next, ILogger<CorsDebuggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log request details for CORS debugging
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var origin = context.Request.Headers.Origin.FirstOrDefault();
            var method = context.Request.Method;
            var path = context.Request.Path;

            _logger.LogDebug("CORS Debug - Origin: {Origin}, Method: {Method}, Path: {Path}", 
                origin, method, path);

            if (method == "OPTIONS")
            {
                _logger.LogDebug("CORS Debug - Preflight request detected");
            }
        }

        await _next(context);

        // Log response headers for CORS debugging
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var corsHeaders = context.Response.Headers
                .Where(h => h.Key.StartsWith("Access-Control-", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            if (corsHeaders.Any())
            {
                _logger.LogDebug("CORS Debug - Response headers: {@CorsHeaders}", corsHeaders);
            }
            else
            {
                _logger.LogDebug("CORS Debug - No CORS headers in response");
            }
        }
    }
}

/// <summary>
/// Extension method to register the CorsDebuggingMiddleware
/// </summary>
public static class CorsDebuggingMiddlewareExtensions
{
    public static IApplicationBuilder UseCorsDebugging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorsDebuggingMiddleware>();
    }
}
