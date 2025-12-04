using Dungeon.Application.Models;

namespace Dungeon.Application.Interfaces;

public interface IPathfinderService
{
    Task<(List<Point> Path, int Distance, bool PathFound)> FindPathAsync(Guid mapId, Point start, Point end, IMapRepository mapRepository);
    Task<(List<Point> Path, int Distance, bool PathFound)> FindPathAsync(DungeonMap map, Point start, Point end);
}