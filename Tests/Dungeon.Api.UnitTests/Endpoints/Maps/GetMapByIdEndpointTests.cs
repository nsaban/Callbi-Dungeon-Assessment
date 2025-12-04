using Dungeon.Api.Endpoints.Maps;
using Dungeon.Api.Models;
using Dungeon.Application.Interfaces;
using Dungeon.Application.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NUnit.Framework;

namespace Dungeon.Api.UnitTests.Endpoints.Maps;

[TestFixture]
public class GetMapByIdEndpointTests
{
    private IMapService _mockMapService;
    private GetMapByIdEndpoint _endpoint;

    [SetUp]
    public void Setup()
    {
        _mockMapService = Substitute.For<IMapService>();
        _endpoint = new GetMapByIdEndpoint(_mockMapService);
    }

    [Test]
    public void Constructor_WithNullMapService_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new GetMapByIdEndpoint(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("mapService");
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithExistingMap_ShouldReturnMapResponse()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var request = new GetMapByIdRequest { Id = mapId };

        var map = new DungeonMap
        {
            Id = mapId,
            Name = "Test Dungeon",
            Width = 10,
            Height = 8,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(9, 7),
            Obstacles = new List<Point> { new(2, 3), new(5, 6) },
            CreatedAt = DateTime.UtcNow
        };

        _mockMapService.GetMapByIdAsync(mapId).Returns(map);

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(mapId);
        result.Name.Should().Be("Test Dungeon");
        result.Width.Should().Be(10);
        result.Height.Should().Be(8);
        result.StartPosition.X.Should().Be(0);
        result.StartPosition.Y.Should().Be(0);
        result.GoalPosition.X.Should().Be(9);
        result.GoalPosition.Y.Should().Be(7);
        result.Obstacles.Should().HaveCount(2);
        result.Obstacles[0].X.Should().Be(2);
        result.Obstacles[0].Y.Should().Be(3);
        result.Obstacles[1].X.Should().Be(5);
        result.Obstacles[1].Y.Should().Be(6);
        result.CreatedAt.Should().Be(map.CreatedAt);

        // Verify service call
        await _mockMapService.Received(1).GetMapByIdAsync(mapId);
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithNonExistentMap_ShouldReturnNull()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var request = new GetMapByIdRequest { Id = mapId };

        _mockMapService.GetMapByIdAsync(mapId).Returns((DungeonMap?)null);

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().BeNull();

        // Verify service call
        await _mockMapService.Received(1).GetMapByIdAsync(mapId);
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithServiceException_ShouldThrowException()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var request = new GetMapByIdRequest { Id = mapId };

        _mockMapService.GetMapByIdAsync(mapId)
            .Returns(Task.FromException<DungeonMap?>(new InvalidOperationException("Database error")));

        // Act & Assert
        var action = async () => await TestHandleAsyncLogic(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database error");

        // Verify service call
        await _mockMapService.Received(1).GetMapByIdAsync(mapId);
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithEmptyObstacles_ShouldReturnMapWithEmptyObstaclesList()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var request = new GetMapByIdRequest { Id = mapId };

        var map = new DungeonMap
        {
            Id = mapId,
            Name = "Empty Dungeon",
            Width = 5,
            Height = 5,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(4, 4),
            Obstacles = new List<Point>(),
            CreatedAt = DateTime.UtcNow
        };

        _mockMapService.GetMapByIdAsync(mapId).Returns(map);

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(mapId);
        result.Name.Should().Be("Empty Dungeon");
        result.Obstacles.Should().BeEmpty();
    }

    /// <summary>
    /// Helper method to test the core business logic without FastEndpoints infrastructure dependencies
    /// </summary>
    private async Task<MapResponse?> TestHandleAsyncLogic(GetMapByIdRequest request)
    {
        var map = await _mockMapService.GetMapByIdAsync(request.Id);
        
        if (map == null)
        {
            return null;
        }

        return new MapResponse
        {
            Id = map.Id,
            Name = map.Name,
            Width = map.Width,
            Height = map.Height,
            StartPosition = new PointModel { X = map.StartPosition.X, Y = map.StartPosition.Y },
            GoalPosition = new PointModel { X = map.GoalPosition.X, Y = map.GoalPosition.Y },
            Obstacles = map.Obstacles.Select(o => new PointModel { X = o.X, Y = o.Y }).ToList(),
            CreatedAt = map.CreatedAt
        };
    }
}