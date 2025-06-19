using System.Text;
using System.Text.Json;

using Application.Common.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IDistributedCache _cache;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(IDistributedCache cache, ILogger<WeatherForecastController> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> GetAsync()
    {
        var cachedForecast = await _cache.GetAsync("forecast");

        if (cachedForecast is null)
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            await _cache.SetAsync("forecast", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(forecast)), new()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(10)
            });

            return this.Ok(forecast);
        }

        return this.Ok(JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(cachedForecast));
    }
}
