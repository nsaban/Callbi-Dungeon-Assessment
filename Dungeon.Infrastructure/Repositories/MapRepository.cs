using Dungeon.Application.Interfaces;
using Dungeon.Application.Models;
using Dungeon.Infrastructure.Database;
using Dungeon.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dungeon.Infrastructure.Repositories;

public class MapRepository : IMapRepository
{
    private readonly DungeonDbContext _context;

    public MapRepository(DungeonDbContext context)
    {
        _context = context;
    }

    public async Task<DungeonMap> CreateMapAsync(DungeonMap map)
    {
        var entity = new DungeonMapEntity
        {
            Id = map.Id,
            Name = map.Name,
            Width = map.Width,
            Height = map.Height,
            StartX = map.StartPosition!.X,
            StartY = map.StartPosition!.Y,
            GoalX = map.GoalPosition!.X,
            GoalY = map.GoalPosition!.Y,
            CreatedAt = map.CreatedAt
        };

        // Add obstacles as separate entities
        foreach (var obstacle in map.Obstacles)
        {
            entity.Obstacles.Add(new ObstacleEntity
            {
                Id = Guid.NewGuid(),
                X = obstacle.X,
                Y = obstacle.Y,
                DungeonMapId = entity.Id
            });
        }

        _context.DungeonMaps.Add(entity);
        await _context.SaveChangesAsync();

        return map;
    }

    public async Task<DungeonMap?> GetMapByIdAsync(Guid id)
    {
        var entity = await _context.DungeonMaps
            .Include(m => m.Obstacles)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (entity == null)
            return null;

        return MapEntityToDomain(entity);
    }

    public async Task<IEnumerable<DungeonMap>> GetAllMapsAsync()
    {
        var entities = await _context.DungeonMaps
            .Include(m => m.Obstacles)
            .ToListAsync();
        return entities.Select(MapEntityToDomain);
    }



    private static DungeonMap MapEntityToDomain(DungeonMapEntity entity)
    {
        var obstacles = entity.Obstacles
            .Select(o => new Point(o.X, o.Y))
            .ToList();

        return new DungeonMap
        {
            Id = entity.Id,
            Name = entity.Name,
            Width = entity.Width,
            Height = entity.Height,
            StartPosition = new Point(entity.StartX, entity.StartY),
            GoalPosition = new Point(entity.GoalX, entity.GoalY),
            Obstacles = obstacles,
            CreatedAt = entity.CreatedAt
        };
    }
}