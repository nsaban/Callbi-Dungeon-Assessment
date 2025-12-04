namespace Dungeon.Application.Models;

public class DungeonMap
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public Point? StartPosition { get; set; }
    public Point? GoalPosition { get; set; }
    public List<Point> Obstacles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}