using GameOfLife.Middleware;
using GameOfLife.Models.Configuration;
using GameOfLife.Persistence;
using GameOfLife.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Get CORS config
var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>();
if (corsSettings?.AllowedOrigins == null || corsSettings.AllowedOrigins.Length == 0)
{
    throw new InvalidOperationException("CORS settings are not configured correctly");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsSettings.PolicyName,
        policy =>
        {
            policy.WithOrigins(corsSettings.AllowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Conway's Game of Life API",
        Version = "v1",
        Description = "An API for simulating Conway's Game of Life with persistent board states."
    });
});

var connectionString = builder.Configuration.GetConnectionString("GameOfLifeDb");
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IGameOfLifeService, GameOfLifeService>();
builder.Services.AddScoped<IBoardRepository, BoardRepository>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsSettings.PolicyName);

app.UseHttpsRedirection();

app.MapControllers();

// Apply database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    dbContext.Database.Migrate();
}

app.Run();