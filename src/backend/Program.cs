using GameOfLife.Middleware;
using GameOfLife.Models;
using GameOfLife.Persistence;
using GameOfLife.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // frontend origin
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services for API controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Conway's Game of Life API",
        Version = "v1",
        Description = "An API for simulating Conway's Game of Life with persistent board states."
    });
});

// Configure EF Core with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("GameOfLifeDb");
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register application services and repositories
builder.Services.AddScoped<IGameOfLifeService, GameOfLifeService>();
builder.Services.AddScoped<IBoardRepository, BoardRepository>();

var app = builder.Build();

// Use custom exception handling middleware for all environments
app.UseMiddleware<ErrorHandlingMiddleware>();

// Enable Swagger UI only in development
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowReactDev");

app.UseHttpsRedirection();

app.MapControllers();

// Automatically apply database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    dbContext.Database.Migrate();
}

app.Run();