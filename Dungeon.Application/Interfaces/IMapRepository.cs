using Dungeon.Application.Models;

namespace Dungeon.Application.Interfaces;

public interface IMapRepository
{
    Task<DungeonMap> CreateMapAsync(DungeonMap map);
    Task<DungeonMap?> GetMapByIdAsync(Guid id);
    Task<IEnumerable<DungeonMap>> GetAllMapsAsync();
}