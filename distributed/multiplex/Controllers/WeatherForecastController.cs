namespace multiplex.Controllers
{
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using StackExchange.Redis;

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> logger;
        private readonly IDatabase redis;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IDatabase redis
        )
        {
            this.logger = logger;
            this.redis = redis;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var result = await redis.StringGetAsync("GetWeatherForecast");

            if (!result.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(result!)!;
            }

            var dataToCache = Enumerable.Range(1, 5).Select(
                    index => new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    }
                )
                .ToArray();

            await redis.StringSetAsync(
                "GetWeatherForecast",
                new RedisValue(JsonSerializer.Serialize(dataToCache)),
                TimeSpan.FromSeconds(10)
            );

            return dataToCache;
        }
    }
}