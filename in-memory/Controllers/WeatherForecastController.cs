using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace in_memory.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMemoryCache memoryCache;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMemoryCache memoryCache)
    {
        _logger = logger;
        this.memoryCache = memoryCache;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        if (!memoryCache.TryGetValue("GetWeatherForecast", out WeatherForecast[] cacheValue))
        {
            cacheValue = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
                .ToArray(); ;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(10));

            memoryCache.Set("GetWeatherForecast", cacheValue, cacheEntryOptions);
        }

        return cacheValue;
    }
}
