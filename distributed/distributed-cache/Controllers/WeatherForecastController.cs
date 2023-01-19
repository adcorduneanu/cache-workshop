using Microsoft.AspNetCore.Mvc;

namespace distributed.Controllers;

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> logger;
    private readonly IDistributedCache distributedCache;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedCache distributedCache)
    {
        this.logger = logger;
        this.distributedCache = distributedCache;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        var cachedData = await distributedCache.GetAsync("GetWeatherForecast");
        if (cachedData != null)
        {
            using MemoryStream ms = new MemoryStream(cachedData);
            var deserializedData = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(ms);
            if (deserializedData != null)
            {
                return deserializedData;
            }
        }

        var dataToCache = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
            .ToArray();

        await distributedCache.SetAsync(
            "GetWeatherForecast",
            JsonSerializer.SerializeToUtf8Bytes(dataToCache),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(10)
            }
        );

        return dataToCache;
    }
}
