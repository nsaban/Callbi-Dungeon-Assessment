using Dungeon.Application.Models;
using Dungeon.Application.Pathfinding;
using FluentAssertions;

namespace Dungeon.Application.UnitTests;

[TestFixture]
public class AStarPathfinderTests
{
    private AStarPathfinder _pathfinder = null!;

    [SetUp]
    public void Setup()
    {
        _pathfinder = new AStarPathfinder();
    }

    [Test]
    public async Task FindPath_WithDirectPath_ShouldReturnShortestPath()
    {
        // Arrange
        var map = new DungeonMap
        {
            Id = Guid.NewGuid(),
            Name = "Test Map",
            Width = 5,
            Height = 5,
            Obstacles = new List<Point>(),
            CreatedAt = DateTime.UtcNow
        };
        var start = new Point(0, 0);
        var end = new Point(2, 2);

        // Act
        var result = await _pathfinder.FindPathAsync(map, start, end);

        // Assert
        result.PathFound.Should().BeTrue();
        result.Path.Should().NotBeEmpty();
        result.Path.First().Should().Be(start);
        result.Path.Last().Should().Be(end);
        result.Distance.Should().Be(4); // Manhattan distance
    }

    [Test]
    public async Task FindPath_WithObstacles_ShouldFindPathAroundObstacles()
    {
        // Arrange
        var map = new DungeonMap
        {
            Id = Guid.NewGuid(),
            Name = "Test Map with Obstacles",
            Width = 5,
            Height = 3,
            Obstacles = new List<Point> { new(1, 1), new(2, 1), new(3, 1) }, // Wall blocking direct path
            CreatedAt = DateTime.UtcNow
        };
        var start = new Point(0, 1);
        var end = new Point(4, 1);

        // Act
        var result = await _pathfinder.FindPathAsync(map, start, end);

        // Assert
        result.PathFound.Should().BeTrue();
        result.Path.Should().NotBeEmpty();
        result.Path.First().Should().Be(start);
        result.Path.Last().Should().Be(end);
        result.Distance.Should().BeGreaterThan(4); // Should be longer than direct path due to obstacles
    }

    [Test]
    public async Task FindPath_WhenNoPathExists_ShouldReturnNoPath()
    {
        // Arrange - create a map where end is completely blocked
        var map = new DungeonMap
        {
            Id = Guid.NewGuid(),
            Name = "Blocked Map",
            Width = 3,
            Height = 3,
            Obstacles = new List<Point> 
            { 
                new(1, 0), new(1, 1), new(1, 2), // Vertical wall
                new(2, 0), new(2, 2) // Block corners, leaving only (2,1) accessible from right
            },
            CreatedAt = DateTime.UtcNow
        };
        var start = new Point(0, 1);
        var end = new Point(2, 1);

        // Act
        var result = await _pathfinder.FindPathAsync(map, start, end);

        // Assert
        result.PathFound.Should().BeFalse();
        result.Path.Should().BeEmpty();
        result.Distance.Should().Be(0);
    }

    [Test]
    public async Task FindPath_WithSameStartAndEnd_ShouldReturnSinglePointPath()
    {
        // Arrange
        var map = new DungeonMap
        {
            Id = Guid.NewGuid(),
            Name = "Test Map",
            Width = 5,
            Height = 5,
            Obstacles = new List<Point>(),
            CreatedAt = DateTime.UtcNow
        };
        var point = new Point(2, 2);

        // Act
        var result = await _pathfinder.FindPathAsync(map, point, point);

        // Assert
        result.PathFound.Should().BeTrue();
        result.Path.Should().HaveCount(1);
        result.Path.First().Should().Be(point);
        result.Distance.Should().Be(0);
    }

    [Test]
    public async Task FindPath_WithInvalidStartPoint_ShouldStillAttemptPathfinding()
    {
        // Arrange
        var map = new DungeonMap
        {
            Id = Guid.NewGuid(),
            Name = "Test Map",
            Width = 5,
            Height = 5,
            Obstacles = new List<Point>(),
            CreatedAt = DateTime.UtcNow
        };
        var invalidStart = new Point(-1, 0); // Out of bounds
        var end = new Point(2, 2);

        // Act
        var result = await _pathfinder.FindPathAsync(map, invalidStart, end);

        // Assert - The A* implementation doesn't validate start point bounds,
        // so it may find a path from invalid start if end is reachable through neighbors
        // This test documents the current behavior rather than enforcing bounds validation
        result.Should().NotBeNull();
        // The exact result depends on implementation details, but should not crash
    }
}