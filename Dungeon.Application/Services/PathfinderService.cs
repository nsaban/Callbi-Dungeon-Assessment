using Dungeon.Application.Interfaces;
using Dungeon.Application.Models;
using Dungeon.Application.Exceptions;

namespace Dungeon.Application.Services;

public class PathfinderService : IPathfinderService
{
    private readonly IAStarPathfinder _pathfinder;

    public PathfinderService(IAStarPathfinder pathfinder)
    {
        _pathfinder = pathfinder;
    }

    public async Task<(List<Point> Path, int Distance, bool PathFound)> FindPathAsync(
        Guid mapId, Point start, Point end, IMapRepository mapRepository)
    {
        var map = await mapRepository.GetMapByIdAsync(mapId);
        if (map == null)
            throw new MapNotFoundException($"Map with id {mapId} not found");

        // Validate start and end points are within bounds
        if (start.X < 0 || start.X >= map.Width || start.Y < 0 || start.Y >= map.Height)
            throw new ArgumentException("Start point is out of bounds");

        if (end.X < 0 || end.X >= map.Width || end.Y < 0 || end.Y >= map.Height)
            throw new ArgumentException("End point is out of bounds");

        // Check if start or end points are obstacles
        if (map.Obstacles.Contains(start))
            throw new ArgumentException("Start point cannot be an obstacle");

        if (map.Obstacles.Contains(end))
            throw new ArgumentException("End point cannot be an obstacle");

        return await _pathfinder.FindPathAsync(map, start, end);
    }

    public async Task<(List<Point> Path, int Distance, bool PathFound)> FindPathAsync(
        DungeonMap map, Point start, Point end)
    {
        // Validate start and end points are within bounds
        if (start.X < 0 || start.X >= map.Width || start.Y < 0 || start.Y >= map.Height)
            throw new ArgumentException("Start point is out of bounds");

        if (end.X < 0 || end.X >= map.Width || end.Y < 0 || end.Y >= map.Height)
            throw new ArgumentException("End point is out of bounds");

        // Check if start or end points are obstacles
        if (map.Obstacles.Contains(start))
            throw new ArgumentException("Start point cannot be an obstacle");

        if (map.Obstacles.Contains(end))
            throw new ArgumentException("End point cannot be an obstacle");

        return await _pathfinder.FindPathAsync(map, start, end);
    }
}