using Dungeon.Application.Models;
using Dungeon.Application.Pathfinding;
using FluentAssertions;

namespace Dungeon.Application.UnitTests;

[TestFixture]
public class GridHelpersTests
{
    [Test]
    public void CalculateDistance_ShouldReturnManhattanDistance()
    {
        // Arrange
        var pointA = new Point(0, 0);
        var pointB = new Point(3, 4);

        // Act
        var distance = GridHelpers.CalculateDistance(pointA, pointB);

        // Assert
        distance.Should().Be(7); // |3-0| + |4-0| = 3 + 4 = 7
    }

    [Test]
    public void CalculateDistance_WithSamePoints_ShouldReturnZero()
    {
        // Arrange
        var point = new Point(5, 5);

        // Act
        var distance = GridHelpers.CalculateDistance(point, point);

        // Assert
        distance.Should().Be(0);
    }

    [Test]
    public void CalculateDistance_WithNegativeCoordinates_ShouldReturnCorrectDistance()
    {
        // Arrange
        var pointA = new Point(-2, -3);
        var pointB = new Point(1, 2);

        // Act
        var distance = GridHelpers.CalculateDistance(pointA, pointB);

        // Assert
        distance.Should().Be(8); // |1-(-2)| + |2-(-3)| = 3 + 5 = 8
    }

    [Test]
    public void GetNeighbors_InMiddleOfGrid_ShouldReturnFourNeighbors()
    {
        // Arrange
        var center = new Point(2, 2);
        var width = 5;
        var height = 5;

        // Act
        var neighbors = GridHelpers.GetNeighbors(center, width, height).ToList();

        // Assert
        neighbors.Should().HaveCount(4);
        neighbors.Should().Contain(new Point(3, 2)); // Right
        neighbors.Should().Contain(new Point(1, 2)); // Left
        neighbors.Should().Contain(new Point(2, 3)); // Up
        neighbors.Should().Contain(new Point(2, 1)); // Down
    }

    [Test]
    public void GetNeighbors_AtTopLeftCorner_ShouldReturnTwoNeighbors()
    {
        // Arrange
        var corner = new Point(0, 0);
        var width = 5;
        var height = 5;

        // Act
        var neighbors = GridHelpers.GetNeighbors(corner, width, height).ToList();

        // Assert
        neighbors.Should().HaveCount(2);
        neighbors.Should().Contain(new Point(1, 0)); // Right
        neighbors.Should().Contain(new Point(0, 1)); // Up
    }

    [Test]
    public void GetNeighbors_AtBottomRightCorner_ShouldReturnTwoNeighbors()
    {
        // Arrange
        var width = 5;
        var height = 5;
        var corner = new Point(width - 1, height - 1); // (4, 4)

        // Act
        var neighbors = GridHelpers.GetNeighbors(corner, width, height).ToList();

        // Assert
        neighbors.Should().HaveCount(2);
        neighbors.Should().Contain(new Point(3, 4)); // Left
        neighbors.Should().Contain(new Point(4, 3)); // Down
    }

    [Test]
    public void GetNeighbors_AtEdge_ShouldReturnThreeNeighbors()
    {
        // Arrange
        var edge = new Point(2, 0); // Top edge
        var width = 5;
        var height = 5;

        // Act
        var neighbors = GridHelpers.GetNeighbors(edge, width, height).ToList();

        // Assert
        neighbors.Should().HaveCount(3);
        neighbors.Should().Contain(new Point(3, 0)); // Right
        neighbors.Should().Contain(new Point(1, 0)); // Left
        neighbors.Should().Contain(new Point(2, 1)); // Up (down from top edge)
    }

    [Test]
    public void GetNeighbors_WithSingleCellGrid_ShouldReturnNoNeighbors()
    {
        // Arrange
        var center = new Point(0, 0);
        var width = 1;
        var height = 1;

        // Act
        var neighbors = GridHelpers.GetNeighbors(center, width, height).ToList();

        // Assert
        neighbors.Should().BeEmpty();
    }
}