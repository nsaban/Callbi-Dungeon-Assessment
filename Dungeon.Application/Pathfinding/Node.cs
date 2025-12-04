using Dungeon.Application.Models;

namespace Dungeon.Application.Pathfinding;

public class Node
{
    public Point Position { get; set; }
    public int GCost { get; set; } // Distance from start
    public int HCost { get; set; } // Distance to end
    public int FCost => GCost + HCost; // Total cost
    public Node? Parent { get; set; }

    public Node(Point position)
    {
        Position = position;
    }
}