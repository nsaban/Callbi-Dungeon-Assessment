namespace Dungeon.Api.Models;

public class PathResponse
{
    public List<PointModel> Path { get; set; } = new();
    public int Distance { get; set; }
    public bool PathFound { get; set; }
}