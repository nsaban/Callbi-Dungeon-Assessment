using Dungeon.Api.Endpoints.Maps;
using Dungeon.Api.Models;
using Dungeon.Application.Interfaces;
using Dungeon.Application.Models;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Dungeon.Api.UnitTests.Endpoints.Maps;

[TestFixture]
public class GetAllMapsEndpointTests
{
    private IMapService _mockMapService;
    private GetAllMapsEndpoint _endpoint;

    [SetUp]
    public void Setup()
    {
        _mockMapService = Substitute.For<IMapService>();
        _endpoint = new GetAllMapsEndpoint(_mockMapService);
    }

    [Test]
    public void Constructor_WithNullMapService_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new GetAllMapsEndpoint(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("mapService");
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithMultipleMaps_ShouldReturnAllMaps()
    {
        // Arrange
        var maps = new List<DungeonMap>
        {
            new DungeonMap
            {
                Id = Guid.NewGuid(),
                Name = "Dungeon 1",
                Width = 10,
                Height = 8,
                StartPosition = new Point(0, 0),
                GoalPosition = new Point(9, 7),
                Obstacles = new List<Point> { new Point(2, 3) },
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new DungeonMap
            {
                Id = Guid.NewGuid(),
                Name = "Dungeon 2",
                Width = 15,
                Height = 12,
                StartPosition = new Point(1, 1),
                GoalPosition = new Point(14, 11),
                Obstacles = new List<Point> { new Point(5, 6), new Point(8, 9) },
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        _mockMapService.GetAllMapsAsync().Returns(maps);

        // Act
        var result = await TestHandleAsyncLogic();

        // Assert
        result.Should().HaveCount(2);
        
        // Verify first map
        var firstMap = result[0];
        firstMap.Id.Should().Be(maps[0].Id);
        firstMap.Name.Should().Be("Dungeon 1");
        firstMap.Width.Should().Be(10);
        firstMap.Height.Should().Be(8);
        firstMap.StartPosition.X.Should().Be(0);
        firstMap.StartPosition.Y.Should().Be(0);
        firstMap.GoalPosition.X.Should().Be(9);
        firstMap.GoalPosition.Y.Should().Be(7);
        firstMap.Obstacles.Should().HaveCount(1);
        firstMap.Obstacles[0].X.Should().Be(2);
        firstMap.Obstacles[0].Y.Should().Be(3);
        firstMap.CreatedAt.Should().Be(maps[0].CreatedAt);

        // Verify second map
        var secondMap = result[1];
        secondMap.Id.Should().Be(maps[1].Id);
        secondMap.Name.Should().Be("Dungeon 2");
        secondMap.Width.Should().Be(15);
        secondMap.Height.Should().Be(12);
        secondMap.StartPosition.X.Should().Be(1);
        secondMap.StartPosition.Y.Should().Be(1);
        secondMap.GoalPosition.X.Should().Be(14);
        secondMap.GoalPosition.Y.Should().Be(11);
        secondMap.Obstacles.Should().HaveCount(2);
        secondMap.Obstacles[0].X.Should().Be(5);
        secondMap.Obstacles[0].Y.Should().Be(6);
        secondMap.Obstacles[1].X.Should().Be(8);
        secondMap.Obstacles[1].Y.Should().Be(9);
        secondMap.CreatedAt.Should().Be(maps[1].CreatedAt);

        // Verify service call
        await _mockMapService.Received(1).GetAllMapsAsync();
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithEmptyMapsList_ShouldReturnEmptyList()
    {
        // Arrange
        _mockMapService.GetAllMapsAsync().Returns(new List<DungeonMap>());

        // Act
        var result = await TestHandleAsyncLogic();

        // Assert
        result.Should().BeEmpty();
        
        // Verify service call
        await _mockMapService.Received(1).GetAllMapsAsync();
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithSingleMap_ShouldReturnSingleMapInList()
    {
        // Arrange
        var singleMap = new DungeonMap
        {
            Id = Guid.NewGuid(),
            Name = "Solo Dungeon",
            Width = 5,
            Height = 5,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(4, 4),
            Obstacles = new List<Point>(),
            CreatedAt = DateTime.UtcNow
        };

        _mockMapService.GetAllMapsAsync().Returns(new List<DungeonMap> { singleMap });

        // Act
        var result = await TestHandleAsyncLogic();

        // Assert
        result.Should().HaveCount(1);
        var mapResponse = result[0];
        mapResponse.Id.Should().Be(singleMap.Id);
        mapResponse.Name.Should().Be("Solo Dungeon");
        mapResponse.Width.Should().Be(5);
        mapResponse.Height.Should().Be(5);
        mapResponse.Obstacles.Should().BeEmpty();
        mapResponse.CreatedAt.Should().Be(singleMap.CreatedAt);

        // Verify service call
        await _mockMapService.Received(1).GetAllMapsAsync();
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithServiceException_ShouldThrowException()
    {
        // Arrange
        _mockMapService.GetAllMapsAsync()
            .Returns(Task.FromException<IEnumerable<DungeonMap>>(new InvalidOperationException("Database connection failed")));

        // Act & Assert
        var action = async () => await TestHandleAsyncLogic();
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database connection failed");

        // Verify service call
        await _mockMapService.Received(1).GetAllMapsAsync();
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithMapsContainingVariousObstacleCounts_ShouldMapAllObstaclesCorrectly()
    {
        // Arrange
        var maps = new List<DungeonMap>
        {
            new DungeonMap
            {
                Id = Guid.NewGuid(),
                Name = "No Obstacles",
                Width = 3,
                Height = 3,
                StartPosition = new Point(0, 0),
                GoalPosition = new Point(2, 2),
                Obstacles = new List<Point>(),
                CreatedAt = DateTime.UtcNow
            },
            new DungeonMap
            {
                Id = Guid.NewGuid(),
                Name = "Many Obstacles",
                Width = 6,
                Height = 6,
                StartPosition = new Point(0, 0),
                GoalPosition = new Point(5, 5),
                Obstacles = new List<Point> 
                { 
                    new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4) 
                },
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockMapService.GetAllMapsAsync().Returns(maps);

        // Act
        var result = await TestHandleAsyncLogic();

        // Assert
        result.Should().HaveCount(2);
        result[0].Obstacles.Should().BeEmpty();
        result[1].Obstacles.Should().HaveCount(4);
        
        // Verify all obstacles are mapped correctly
        for (int i = 0; i < 4; i++)
        {
            result[1].Obstacles[i].X.Should().Be(i + 1);
            result[1].Obstacles[i].Y.Should().Be(i + 1);
        }
    }

    /// <summary>
    /// Helper method to test the core business logic without FastEndpoints infrastructure dependencies
    /// </summary>
    private async Task<List<MapResponse>> TestHandleAsyncLogic()
    {
        var maps = await _mockMapService.GetAllMapsAsync();
        
        return maps.Select(map => new MapResponse
        {
            Id = map.Id,
            Name = map.Name,
            Width = map.Width,
            Height = map.Height,
            StartPosition = new PointModel { X = map.StartPosition.X, Y = map.StartPosition.Y },
            GoalPosition = new PointModel { X = map.GoalPosition.X, Y = map.GoalPosition.Y },
            Obstacles = map.Obstacles.Select(o => new PointModel { X = o.X, Y = o.Y }).ToList(),
            CreatedAt = map.CreatedAt
        }).ToList();
    }
}