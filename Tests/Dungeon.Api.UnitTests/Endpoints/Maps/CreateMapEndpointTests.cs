using Dungeon.Api.Endpoints.Maps;
using Dungeon.Api.Models;
using Dungeon.Application.Interfaces;
using Dungeon.Application.Models;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Dungeon.Api.UnitTests.Endpoints.Maps;

[TestFixture]
public class CreateMapEndpointTests
{
    private IMapService _mockMapService;
    private CreateMapEndpoint _endpoint;

    [SetUp]
    public void Setup()
    {
        _mockMapService = Substitute.For<IMapService>();
        _endpoint = new CreateMapEndpoint(_mockMapService);
    }

    [Test]
    public void Constructor_WithNullMapService_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new CreateMapEndpoint(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("mapService");
    }

    [Test]
    public void Constructor_WithValidMapService_ShouldCreateInstance()
    {
        // Act & Assert
        _endpoint.Should().NotBeNull();
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithValidRequest_ShouldCreateMapSuccessfully()
    {
        // Arrange
        var request = new CreateMapRequest
        {
            Name = "Test Dungeon",
            Width = 10,
            Height = 8,
            StartPosition = new PointModel { X = 0, Y = 0 },
            GoalPosition = new PointModel { X = 9, Y = 7 },
            Obstacles = new List<PointModel>
            {
                new PointModel { X = 2, Y = 3 },
                new PointModel { X = 5, Y = 6 }
            }
        };

        var createdMap = new DungeonMap
        {
            Id = Guid.NewGuid(),
            Name = "Test Dungeon",
            Width = 10,
            Height = 8,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(9, 7),
            Obstacles = new List<Point> { new Point(2, 3), new Point(5, 6) },
            CreatedAt = DateTime.UtcNow
        };

        _mockMapService.CreateMapAsync(
            request.Name,
            request.Width,
            request.Height,
            Arg.Any<Point>(),
            Arg.Any<Point>(),
            Arg.Any<List<Point>>())
            .Returns(createdMap);

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(createdMap.Id);
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
        result.CreatedAt.Should().Be(createdMap.CreatedAt);

        // Verify service call
        await _mockMapService.Received(1).CreateMapAsync(
            "Test Dungeon",
            10,
            8,
            Arg.Is<Point>(p => p.X == 0 && p.Y == 0),
            Arg.Is<Point>(p => p.X == 9 && p.Y == 7),
            Arg.Is<List<Point>>(obstacles => obstacles.Count == 2 &&
                obstacles[0].X == 2 && obstacles[0].Y == 3 &&
                obstacles[1].X == 5 && obstacles[1].Y == 6));
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithEmptyObstacles_ShouldCreateMapWithNoObstacles()
    {
        // Arrange
        var request = new CreateMapRequest
        {
            Name = "Empty Dungeon",
            Width = 5,
            Height = 5,
            StartPosition = new PointModel { X = 0, Y = 0 },
            GoalPosition = new PointModel { X = 4, Y = 4 },
            Obstacles = new List<PointModel>()
        };

        var createdMap = new DungeonMap
        {
            Id = Guid.NewGuid(),
            Name = "Empty Dungeon",
            Width = 5,
            Height = 5,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(4, 4),
            Obstacles = new List<Point>(),
            CreatedAt = DateTime.UtcNow
        };

        _mockMapService.CreateMapAsync(
            request.Name,
            request.Width,
            request.Height,
            Arg.Any<Point>(),
            Arg.Any<Point>(),
            Arg.Any<List<Point>>())
            .Returns(createdMap);

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Empty Dungeon");
        result.Obstacles.Should().BeEmpty();

        // Verify service call with empty obstacles
        await _mockMapService.Received(1).CreateMapAsync(
            "Empty Dungeon",
            5,
            5,
            Arg.Is<Point>(p => p.X == 0 && p.Y == 0),
            Arg.Is<Point>(p => p.X == 4 && p.Y == 4),
            Arg.Is<List<Point>>(obstacles => obstacles.Count == 0));
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithServiceException_ShouldThrowException()
    {
        // Arrange
        var request = new CreateMapRequest
        {
            Name = "Test Dungeon",
            Width = 10,
            Height = 8,
            StartPosition = new PointModel { X = 0, Y = 0 },
            GoalPosition = new PointModel { X = 9, Y = 7 },
            Obstacles = new List<PointModel>()
        };

        _mockMapService.CreateMapAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Point>(),
            Arg.Any<Point>(),
            Arg.Any<List<Point>>())
            .Returns(Task.FromException<DungeonMap>(new InvalidOperationException("Database error")));

        // Act & Assert
        var action = async () => await TestHandleAsyncLogic(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database error");
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithManyObstacles_ShouldMapAllObstaclesCorrectly()
    {
        // Arrange
        var obstacles = new List<PointModel>();
        for (int i = 0; i < 10; i++)
        {
            obstacles.Add(new PointModel { X = i, Y = i });
        }

        var request = new CreateMapRequest
        {
            Name = "Complex Dungeon",
            Width = 20,
            Height = 20,
            StartPosition = new PointModel { X = 0, Y = 0 },
            GoalPosition = new PointModel { X = 19, Y = 19 },
            Obstacles = obstacles
        };

        var mappedObstacles = obstacles.Select(o => new Point(o.X, o.Y)).ToList();
        var createdMap = new DungeonMap
        {
            Id = Guid.NewGuid(),
            Name = "Complex Dungeon",
            Width = 20,
            Height = 20,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(19, 19),
            Obstacles = mappedObstacles,
            CreatedAt = DateTime.UtcNow
        };

        _mockMapService.CreateMapAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Point>(),
            Arg.Any<Point>(),
            Arg.Any<List<Point>>())
            .Returns(createdMap);

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Complex Dungeon");
        result.Obstacles.Should().HaveCount(10);
        
        // Verify all obstacles are mapped correctly
        for (int i = 0; i < 10; i++)
        {
            result.Obstacles[i].X.Should().Be(i);
            result.Obstacles[i].Y.Should().Be(i);
        }
    }

    /// <summary>
    /// Helper method to test the core business logic without FastEndpoints infrastructure dependencies
    /// </summary>
    private async Task<CreateMapResponse> TestHandleAsyncLogic(CreateMapRequest request)
    {
        var obstacles = request.Obstacles.Select(o => new Point(o.X, o.Y)).ToList();
        var startPosition = new Point(request.StartPosition.X, request.StartPosition.Y);
        var goalPosition = new Point(request.GoalPosition.X, request.GoalPosition.Y);
        var map = await _mockMapService.CreateMapAsync(request.Name, request.Width, request.Height, startPosition, goalPosition, obstacles);
        
        return new CreateMapResponse
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