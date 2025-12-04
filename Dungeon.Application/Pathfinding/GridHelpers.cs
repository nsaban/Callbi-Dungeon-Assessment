using Dungeon.Application.Models;

namespace Dungeon.Application.Pathfinding;

public static class GridHelpers
{
    public static int CalculateDistance(Point a, Point b)
    {
        var deltaX = Math.Abs(a.X - b.X);
        var deltaY = Math.Abs(a.Y - b.Y);
        
        // Manhattan distance for 4-directional movement
        return deltaX + deltaY;
    }

    public static IEnumerable<Point> GetNeighbors(Point point, int width, int height)
    {
        var neighbors = new[]
        {
            new Point(point.X + 1, point.Y),     // Right
            new Point(point.X - 1, point.Y),     // Left
            new Point(point.X, point.Y + 1),     // Up
            new Point(point.X, point.Y - 1)      // Down
        };

        return neighbors.Where(p => p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height);
    }
}