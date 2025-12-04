using Dungeon.Api.Middleware;
using Dungeon.Application.Interfaces;
using Dungeon.Application.Services;
using Dungeon.Infrastructure.Database;
using Dungeon.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Dungeon.Application.Pathfinding;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// DB - sqlite in ./data/dungeon.db
var dbPath = Path.Combine(AppContext.BaseDirectory, "data", "dungeon.db");
var connectionString = $"Data Source={dbPath}";
builder.Services.AddDbContext<DungeonDbContext>(opt => 
    opt.UseSqlite(connectionString, b => b.MigrationsAssembly("Dungeon.Infrastructure")));

// Add FastEndpoints
builder.Services.AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "Dungeon Assessment API";
            s.Version = "v1";
            s.Description = "API for managing dungeon maps and pathfinding";
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

// Add CORS
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:5173" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Ensure folder for database exists
var dataFolder = Path.Combine(AppContext.BaseDirectory, "data");
Directory.CreateDirectory(dataFolder);

// AutoMapper not needed with FastEndpoints manual mapping

// Add Application Services
builder.Services.AddScoped<IMapRepository, MapRepository>();
builder.Services.AddScoped<IAStarPathfinder, AStarPathfinder>();
builder.Services.AddScoped<IMapService, MapService>();
builder.Services.AddScoped<IPathfinderService, PathfinderService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DungeonDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowReactApp");

app.UseAuthorization();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();