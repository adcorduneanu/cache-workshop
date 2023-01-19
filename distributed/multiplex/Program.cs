using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(
    _ =>
        ConnectionMultiplexer.Connect(
            builder.Configuration.GetSection("Redis:ConnectionString").Get<string>() ??
            throw new InvalidOperationException(nameof(ConnectionMultiplexer))
        )
);
builder.Services.AddSingleton<IDatabase>(
    serviceProvider => serviceProvider.GetService<IConnectionMultiplexer>()?.GetDatabase()
);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

app.Run();