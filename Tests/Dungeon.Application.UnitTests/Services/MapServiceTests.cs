using Dungeon.Application.Interfaces;
using Dungeon.Application.Models;
using Dungeon.Application.Services;
using FluentAssertions;
using NSubstitute;

namespace Dungeon.Application.UnitTests;

[TestFixture]
public class MapServiceTests
{
    private IMapRepository _mockRepository = null!;
    private MapService _mapService = null!;

    [SetUp]
    public void Setup()
    {
        _mockRepository = Substitute.For<IMapRepository>();
        _mapService = new MapService(_mockRepository);
    }

    [Test]
    public async Task CreateMapAsync_ShouldCreateMapWithCorrectProperties()
    {
        // Arrange
        var name = "Test Dungeon";
        var width = 10;
        var height = 8;
        var startPosition = new Point(1, 1);
        var goalPosition = new Point(8, 6);
        var obstacles = new List<Point> { new(2, 3), new(5, 7) };

        var expectedMap = new DungeonMap
        {
            Id = Guid.NewGuid(),
            Name = name,
            Width = width,
            Height = height,
            StartPosition = startPosition,
            GoalPosition = goalPosition,
            Obstacles = obstacles,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.CreateMapAsync(Arg.Any<DungeonMap>())
            .Returns(expectedMap);

        // Act
        var result = await _mapService.CreateMapAsync(name, width, height, startPosition, goalPosition, obstacles);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(name);
        result.Width.Should().Be(width);
        result.Height.Should().Be(height);
        result.StartPosition.Should().Be(startPosition);
        result.GoalPosition.Should().Be(goalPosition);
        result.Obstacles.Should().BeEquivalentTo(obstacles);
        result.Id.Should().NotBe(Guid.Empty);
        
        await _mockRepository.Received(1).CreateMapAsync(Arg.Any<DungeonMap>());
    }

    [Test]
    public async Task GetMapByIdAsync_WhenMapExists_ShouldReturnMap()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var expectedMap = new DungeonMap
        {
            Id = mapId,
            Name = "Test Map",
            Width = 5,
            Height = 5,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(4, 4),
            Obstacles = new List<Point> { new(1, 1) },
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.GetMapByIdAsync(mapId).Returns(expectedMap);

        // Act
        var result = await _mapService.GetMapByIdAsync(mapId);

        // Assert
        result.Should().Be(expectedMap);
        await _mockRepository.Received(1).GetMapByIdAsync(mapId);
    }

    [Test]
    public async Task GetMapByIdAsync_WhenMapDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        _mockRepository.GetMapByIdAsync(mapId).Returns((DungeonMap?)null);

        // Act
        var result = await _mapService.GetMapByIdAsync(mapId);

        // Assert
        result.Should().BeNull();
        await _mockRepository.Received(1).GetMapByIdAsync(mapId);
    }

    [Test]
    public async Task GetAllMapsAsync_ShouldReturnAllMaps()
    {
        // Arrange
        var expectedMaps = new List<DungeonMap>
        {
            new() { Id = Guid.NewGuid(), Name = "Map1", Width = 5, Height = 5, StartPosition = new Point(0, 0), GoalPosition = new Point(4, 4), Obstacles = new List<Point>(), CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Map2", Width = 8, Height = 6, StartPosition = new Point(1, 1), GoalPosition = new Point(6, 4), Obstacles = new List<Point>(), CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.GetAllMapsAsync().Returns(expectedMaps);

        // Act
        var result = await _mapService.GetAllMapsAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedMaps);
        await _mockRepository.Received(1).GetAllMapsAsync();
    }


}
