using Dungeon.Application.Models;

namespace Dungeon.Application.Interfaces;

public interface IMapService
{
    Task<DungeonMap> CreateMapAsync(string name, int width, int height, Point startPosition, Point goalPosition, List<Point> obstacles);
    Task<DungeonMap?> GetMapByIdAsync(Guid id);
    Task<IEnumerable<DungeonMap>> GetAllMapsAsync();
}