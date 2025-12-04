namespace Dungeon.Infrastructure.Entities;

public class DungeonMapEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int GoalX { get; set; }
    public int GoalY { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    public ICollection<ObstacleEntity> Obstacles { get; set; } = new List<ObstacleEntity>();
}