using Dungeon.Application.Models;

namespace Dungeon.Application.Interfaces;

public interface IAStarPathfinder
{
    Task<(List<Point> Path, int Distance, bool PathFound)> FindPathAsync(
        DungeonMap map, Point start, Point end);
}