using Dungeon.Application.Interfaces;
using Dungeon.Application.Models;

namespace Dungeon.Application.Pathfinding;

public class AStarPathfinder : IAStarPathfinder
{
    public async Task<(List<Point> Path, int Distance, bool PathFound)> FindPathAsync(
        DungeonMap map, Point start, Point end)
    {
        return await Task.Run(() => FindPath(map, start, end));
    }

    private (List<Point> Path, int Distance, bool PathFound) FindPath(
        DungeonMap map, Point start, Point end)
    {
        var openSet = new List<Node>();
        var closedSet = new HashSet<Point>();
        var obstacles = new HashSet<Point>(map.Obstacles);

        var startNode = new Node(start);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet.OrderBy(n => n.FCost).ThenBy(n => n.HCost).First();
            openSet.Remove(currentNode);
            closedSet.Add(currentNode.Position);

            if (currentNode.Position.Equals(end))
            {
                var path = ReconstructPath(currentNode);
                return (path, currentNode.GCost, true);
            }

            foreach (var neighborPos in GridHelpers.GetNeighbors(currentNode.Position, map.Width, map.Height))
            {
                if (closedSet.Contains(neighborPos) || obstacles.Contains(neighborPos))
                    continue;

                var neighbor = openSet.FirstOrDefault(n => n.Position.Equals(neighborPos));
                var tentativeGCost = currentNode.GCost + 1; // Cost of moving to neighbor

                if (neighbor == null)
                {
                    neighbor = new Node(neighborPos)
                    {
                        GCost = tentativeGCost,
                        HCost = GridHelpers.CalculateDistance(neighborPos, end),
                        Parent = currentNode
                    };
                    openSet.Add(neighbor);
                }
                else if (tentativeGCost < neighbor.GCost)
                {
                    neighbor.GCost = tentativeGCost;
                    neighbor.Parent = currentNode;
                }
            }
        }

        return (new List<Point>(), 0, false);
    }

    private List<Point> ReconstructPath(Node endNode)
    {
        var path = new List<Point>();
        var currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }
}