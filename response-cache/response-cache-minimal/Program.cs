using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddResponseCaching();
builder.Services.AddControllers(
    options =>
    {
        options.CacheProfiles.Add(
            "Default30",
            new CacheProfile
            {
                Duration = 30
            }
        );
    }
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseResponseCaching();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.Use(
    async (
        context,
        next
    ) =>
    {
        Console.WriteLine(DateTimeOffset.UtcNow);

        await next();
    }
);

app.MapGet(
        "/weatherforecast",
        (
            HttpContext context
        ) =>
        {
            var forecast = Enumerable.Range(1, 5).Select(
                    index =>
                        new WeatherForecast(
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        )
                )
                .ToArray();

            context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(10),
                Public = true,
                NoCache = false,
                NoStore = false
            };

            return forecast;
        }
    )
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

internal record WeatherForecast(
    DateOnly Date,
    int TemperatureC,
    string? Summary
)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}