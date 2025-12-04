using Dungeon.Application.Interfaces;
using Dungeon.Application.Models;
using Dungeon.Application.Services;
using FluentAssertions;
using NSubstitute;

namespace Dungeon.Application.UnitTests;

[TestFixture]
public class PathfinderServiceTests
{
    private IAStarPathfinder _mockPathfinder = null!;
    private IMapRepository _mockMapRepository = null!;
    private PathfinderService _pathfinderService = null!;

    [SetUp]
    public void Setup()
    {
        _mockPathfinder = Substitute.For<IAStarPathfinder>();
        _mockMapRepository = Substitute.For<IMapRepository>();
        _pathfinderService = new PathfinderService(_mockPathfinder);
    }

    [Test]
    public async Task FindPathAsync_WhenMapExists_ShouldReturnPath()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var map = new DungeonMap
        {
            Id = mapId,
            Name = "Test Map",
            Width = 5,
            Height = 5,
            Obstacles = new List<Point>(),
            CreatedAt = DateTime.UtcNow
        };
        var start = new Point(0, 0);
        var end = new Point(2, 2);
        var expectedResult = (Path: new List<Point> { start, new(1, 1), end }, Distance: 4, PathFound: true);

        _mockMapRepository.GetMapByIdAsync(mapId).Returns(map);
        _mockPathfinder.FindPathAsync(map, start, end).Returns(expectedResult);

        // Act
        var result = await _pathfinderService.FindPathAsync(mapId, start, end, _mockMapRepository);

        // Assert
        result.Should().Be(expectedResult);
        await _mockMapRepository.Received(1).GetMapByIdAsync(mapId);
        await _mockPathfinder.Received(1).FindPathAsync(map, start, end);
    }

    [Test]
    public async Task FindPathAsync_WhenMapDoesNotExist_ShouldThrowMapNotFoundException()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var start = new Point(0, 0);
        var end = new Point(2, 2);

        _mockMapRepository.GetMapByIdAsync(mapId).Returns((DungeonMap?)null);

        // Act & Assert
        var act = async () => await _pathfinderService.FindPathAsync(mapId, start, end, _mockMapRepository);
        
        await act.Should().ThrowAsync<Application.Exceptions.MapNotFoundException>()
            .WithMessage($"Map with id {mapId} not found");
        
        await _mockMapRepository.Received(1).GetMapByIdAsync(mapId);
        await _mockPathfinder.DidNotReceiveWithAnyArgs().FindPathAsync(Arg.Any<DungeonMap>(), Arg.Any<Point>(), Arg.Any<Point>());
    }

    [Test]
    public async Task FindPathAsync_WithValidInputs_ShouldCallPathfinderWithCorrectParameters()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var map = new DungeonMap
        {
            Id = mapId,
            Name = "Test Map",
            Width = 10,
            Height = 8,
            Obstacles = new List<Point> { new(3, 3), new(4, 4) },
            CreatedAt = DateTime.UtcNow
        };
        var start = new Point(1, 1);
        var end = new Point(7, 6);
        var pathResult = (Path: new List<Point> { start, end }, Distance: 10, PathFound: true);

        _mockMapRepository.GetMapByIdAsync(mapId).Returns(map);
        _mockPathfinder.FindPathAsync(map, start, end).Returns(pathResult);

        // Act
        var result = await _pathfinderService.FindPathAsync(mapId, start, end, _mockMapRepository);

        // Assert
        result.Should().NotBeNull();
        result.Path.Should().BeEquivalentTo(pathResult.Path);
        result.Distance.Should().Be(pathResult.Distance);
        result.PathFound.Should().Be(pathResult.PathFound);
        
        await _mockPathfinder.Received(1).FindPathAsync(
            Arg.Is<DungeonMap>(m => m.Id == mapId && m.Obstacles.Count == 2), 
            start, 
            end);
    }
}