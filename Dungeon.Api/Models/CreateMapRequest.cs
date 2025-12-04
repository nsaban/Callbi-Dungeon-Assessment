namespace Dungeon.Api.Models;

public class CreateMapRequest
{
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public PointModel StartPosition { get; set; } = new();
    public PointModel GoalPosition { get; set; } = new();
    public List<PointModel> Obstacles { get; set; } = new();
}