namespace Dungeon.Api.Models;

public class MapResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public PointModel StartPosition { get; set; } = new();
    public PointModel GoalPosition { get; set; } = new();
    public List<PointModel> Obstacles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}