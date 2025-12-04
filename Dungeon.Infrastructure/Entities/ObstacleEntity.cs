namespace Dungeon.Infrastructure.Entities;

public class ObstacleEntity
{
    public Guid Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Guid DungeonMapId { get; set; }
    
    // Navigation property
    public DungeonMapEntity DungeonMap { get; set; } = null!;
}