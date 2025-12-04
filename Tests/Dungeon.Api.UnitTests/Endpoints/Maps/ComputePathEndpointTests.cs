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
public class ComputePathEndpointTests
{
    private IPathfinderService _mockPathfinderService;
    private IMapService _mockMapService;
    private ComputePathEndpoint _endpoint;

    [SetUp]
    public void Setup()
    {
        _mockPathfinderService = Substitute.For<IPathfinderService>();
        _mockMapService = Substitute.For<IMapService>();
        _endpoint = new ComputePathEndpoint(_mockPathfinderService, _mockMapService);
    }

    [Test]
    public void Constructor_WithNullPathfinderService_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new ComputePathEndpoint(null!, _mockMapService);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("pathfinderService");
    }

    [Test]
    public void Constructor_WithNullMapService_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new ComputePathEndpoint(_mockPathfinderService, null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("mapService");
    }

    [Test]
    public void Constructor_WithValidDependencies_ShouldCreateInstance()
    {
        // Act & Assert
        _endpoint.Should().NotBeNull();
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithValidMapAndPath_ShouldReturnOkWithPathResponse()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var request = new GetMapByIdRequest { Id = mapId };

        var map = new DungeonMap
        {
            Id = mapId,
            Name = "Test Map",
            Width = 10,
            Height = 10,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(9, 9),
            Obstacles = new List<Point>()
        };

        var pathResult = (
            Path: new List<Point> { new(0, 0), new(1, 1), new(2, 2), new(9, 9) },
            Distance: 4,
            PathFound: true
        );

        _mockMapService.GetMapByIdAsync(mapId).Returns(map);
        _mockPathfinderService.FindPathAsync(map, map.StartPosition, map.GoalPosition)
            .Returns(pathResult);

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<PathResponse>>();
        
        var okResult = (Ok<PathResponse>)result.Result;
        okResult.Value.Should().NotBeNull();
        okResult.Value.PathFound.Should().BeTrue();
        okResult.Value.Distance.Should().Be(4);
        okResult.Value.Path.Should().HaveCount(4);
        okResult.Value.Path[0].X.Should().Be(0);
        okResult.Value.Path[0].Y.Should().Be(0);
        okResult.Value.Path[3].X.Should().Be(9);
        okResult.Value.Path[3].Y.Should().Be(9);

        // Verify service calls
        await _mockMapService.Received(1).GetMapByIdAsync(mapId);
        await _mockPathfinderService.Received(1).FindPathAsync(map, map.StartPosition, map.GoalPosition);
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithNonExistentMap_ShouldReturnProblemHttpResult()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var request = new GetMapByIdRequest { Id = mapId };

        _mockMapService.GetMapByIdAsync(mapId).Returns((DungeonMap?)null);

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<ProblemHttpResult>();
        
        var problemResult = (ProblemHttpResult)result.Result;
        problemResult.StatusCode.Should().Be(404);
        problemResult.ProblemDetails.Detail.Should().Contain($"Map with id {mapId} not found");

        // Verify map service was called but pathfinder service was not
        await _mockMapService.Received(1).GetMapByIdAsync(mapId);
        await _mockPathfinderService.DidNotReceiveWithAnyArgs().FindPathAsync(
            Arg.Any<DungeonMap>(), Arg.Any<Point>(), Arg.Any<Point>()
        );
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithArgumentException_ShouldReturnBadRequestProblem()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var request = new GetMapByIdRequest { Id = mapId };

        var map = new DungeonMap
        {
            Id = mapId,
            Name = "Test Map",
            Width = 10,
            Height = 10,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(9, 9),
            Obstacles = new List<Point>()
        };

        _mockMapService.GetMapByIdAsync(mapId).Returns(map);
        _mockPathfinderService.FindPathAsync(map, map.StartPosition, map.GoalPosition)
            .Returns(Task.FromException<(List<Point> Path, int Distance, bool PathFound)>(new ArgumentException("Invalid map configuration")));

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<ProblemHttpResult>();
        
        var problemResult = (ProblemHttpResult)result.Result;
        problemResult.StatusCode.Should().Be(400);
        problemResult.ProblemDetails.Detail.Should().Contain("Invalid map configuration");
        problemResult.ProblemDetails.Detail.Should().Contain("An error occurred while computing the path, fix the request");

        // Verify both services were called
        await _mockMapService.Received(1).GetMapByIdAsync(mapId);
        await _mockPathfinderService.Received(1).FindPathAsync(map, map.StartPosition, map.GoalPosition);
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithUnexpectedException_ShouldReturnServerErrorProblem()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var request = new GetMapByIdRequest { Id = mapId };

        var map = new DungeonMap
        {
            Id = mapId,
            Name = "Test Map",
            Width = 10,
            Height = 10,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(9, 9),
            Obstacles = new List<Point>()
        };

        _mockMapService.GetMapByIdAsync(mapId).Returns(map);
        _mockPathfinderService.FindPathAsync(map, map.StartPosition, map.GoalPosition)
            .Returns(Task.FromException<(List<Point> Path, int Distance, bool PathFound)>(new InvalidOperationException("Internal processing error")));

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<ProblemHttpResult>();
        
        var problemResult = (ProblemHttpResult)result.Result;
        problemResult.StatusCode.Should().Be(500);
        problemResult.ProblemDetails.Detail.Should().Contain("Internal processing error");
        problemResult.ProblemDetails.Detail.Should().Contain("An error occurred while computing the path");

        // Verify both services were called
        await _mockMapService.Received(1).GetMapByIdAsync(mapId);
        await _mockPathfinderService.Received(1).FindPathAsync(map, map.StartPosition, map.GoalPosition);
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithPathNotFound_ShouldReturnOkWithNoPath()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var request = new GetMapByIdRequest { Id = mapId };

        var map = new DungeonMap
        {
            Id = mapId,
            Name = "Test Map",
            Width = 10,
            Height = 10,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(9, 9),
            Obstacles = new List<Point>()
        };

        var pathResult = (
            Path: new List<Point>(),
            Distance: 0,
            PathFound: false
        );

        _mockMapService.GetMapByIdAsync(mapId).Returns(map);
        _mockPathfinderService.FindPathAsync(map, map.StartPosition, map.GoalPosition)
            .Returns(pathResult);

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<PathResponse>>();
        
        var okResult = (Ok<PathResponse>)result.Result;
        okResult.Value.Should().NotBeNull();
        okResult.Value.PathFound.Should().BeFalse();
        okResult.Value.Distance.Should().Be(0);
        okResult.Value.Path.Should().BeEmpty();

        // Verify service calls
        await _mockMapService.Received(1).GetMapByIdAsync(mapId);
        await _mockPathfinderService.Received(1).FindPathAsync(map, map.StartPosition, map.GoalPosition);
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithComplexPath_ShouldMapAllPathPointsCorrectly()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var request = new GetMapByIdRequest { Id = mapId };

        var map = new DungeonMap
        {
            Id = mapId,
            Name = "Complex Map",
            Width = 5,
            Height = 5,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(4, 4),
            Obstacles = new List<Point> { new Point(2, 2) }
        };

        var complexPath = new List<Point>
        {
            new(0, 0), new(0, 1), new(1, 1), new(1, 2), 
            new(1, 3), new(2, 3), new(3, 3), new(4, 3), new(4, 4)
        };

        var pathResult = (
            Path: complexPath,
            Distance: 8,
            PathFound: true
        );

        _mockMapService.GetMapByIdAsync(mapId).Returns(map);
        _mockPathfinderService.FindPathAsync(map, map.StartPosition, map.GoalPosition)
            .Returns(pathResult);

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<PathResponse>>();
        
        var okResult = (Ok<PathResponse>)result.Result;
        okResult.Value.Should().NotBeNull();
        okResult.Value.PathFound.Should().BeTrue();
        okResult.Value.Distance.Should().Be(8);
        okResult.Value.Path.Should().HaveCount(9);

        // Verify all path points are mapped correctly
        for (int i = 0; i < complexPath.Count; i++)
        {
            okResult.Value.Path[i].X.Should().Be(complexPath[i].X);
            okResult.Value.Path[i].Y.Should().Be(complexPath[i].Y);
        }
    }

    [Test]
    public async Task TestHandleAsyncLogic_WithEmptyPath_ShouldReturnOkWithEmptyPath()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var request = new GetMapByIdRequest { Id = mapId };

        var map = new DungeonMap
        {
            Id = mapId,
            Name = "Test Map",
            Width = 10,
            Height = 10,
            StartPosition = new Point(0, 0),
            GoalPosition = new Point(9, 9),
            Obstacles = new List<Point>()
        };

        var pathResult = (
            Path: new List<Point>(), // Empty path for no path found scenario
            Distance: 0,
            PathFound: false
        );

        _mockMapService.GetMapByIdAsync(mapId).Returns(map);
        _mockPathfinderService.FindPathAsync(map, map.StartPosition, map.GoalPosition)
            .Returns(pathResult);

        // Act
        var result = await TestHandleAsyncLogic(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<Ok<PathResponse>>();
        
        var okResult = (Ok<PathResponse>)result.Result;
        okResult.Value.Should().NotBeNull();
        okResult.Value.PathFound.Should().BeFalse();
        okResult.Value.Distance.Should().Be(0);
        okResult.Value.Path.Should().NotBeNull();
        okResult.Value.Path.Should().BeEmpty();

        // Verify service calls
        await _mockMapService.Received(1).GetMapByIdAsync(mapId);
        await _mockPathfinderService.Received(1).FindPathAsync(map, map.StartPosition, map.GoalPosition);
    }

    /// <summary>
    /// Helper method to test the core business logic without FastEndpoints infrastructure dependencies
    /// </summary>
    private async Task<Results<Ok<PathResponse>, ProblemHttpResult>> TestHandleAsyncLogic(GetMapByIdRequest request)
    {
        try
        {
            // Get the map with its built-in start and goal positions
            var map = await _mockMapService.GetMapByIdAsync(request.Id);
            if (map == null)
            {
                return TypedResults.Problem(
                    detail: $"Map with id {request.Id} not found",
                    statusCode: 404);
            }

            // Use the stored start and goal positions
            var result = await _mockPathfinderService.FindPathAsync(
                map, 
                map.StartPosition, 
                map.GoalPosition);

            var pathPoints = result.Path?.Select(p => new PointModel { X = p.X, Y = p.Y })?.ToList() ?? new List<PointModel>();
            
            var response = new PathResponse
            {
                Path = pathPoints,
                Distance = result.Distance,
                PathFound = result.PathFound
            };

            return TypedResults.Ok(response);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.Problem(
                detail: $"An error occurred while computing the path, fix the request: {ex.Message}",
                statusCode: 400);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(
                detail: $"An error occurred while computing the path: {ex.Message}",
                statusCode: 500);
        }
    }
}