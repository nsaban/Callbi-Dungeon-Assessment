using Dungeon.Application.Interfaces;
using Dungeon.Application.Models;
using Dungeon.Application.Exceptions;

namespace Dungeon.Application.Services;

public class MapService : IMapService
{
    private readonly IMapRepository _mapRepository;

    public MapService(IMapRepository mapRepository)
    {
        _mapRepository = mapRepository;
    }

    public async Task<DungeonMap> CreateMapAsync(string name, int width, int height, Point startPosition, Point goalPosition, List<Point> obstacles)
    {
        if (width <= 0 || height <= 0)
            throw new InvalidMapException("Map dimensions must be positive");

        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidMapException("Map name cannot be empty");

        var map = new DungeonMap
        {
            Id = Guid.NewGuid(),
            Name = name,
            Width = width,
            Height = height,
            StartPosition = startPosition,
            GoalPosition = goalPosition,
            Obstacles = obstacles ?? new List<Point>(),
            CreatedAt = DateTime.UtcNow
        };

        return await _mapRepository.CreateMapAsync(map);
    }

    public async Task<DungeonMap?> GetMapByIdAsync(Guid id)
    {
        return await _mapRepository.GetMapByIdAsync(id);
    }

    public async Task<IEnumerable<DungeonMap>> GetAllMapsAsync()
    {
        return await _mapRepository.GetAllMapsAsync();
    }


}