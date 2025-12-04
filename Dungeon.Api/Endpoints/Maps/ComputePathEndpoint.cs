using Dungeon.Api.Models;
using Dungeon.Application.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Dungeon.Api.Endpoints.Maps;

public class ComputePathEndpoint : Endpoint<GetMapByIdRequest, Results<Ok<PathResponse>, ProblemHttpResult>>
{
    private readonly IPathfinderService _pathfinderService;
    private readonly IMapService _mapService;

    public ComputePathEndpoint(IPathfinderService pathfinderService, IMapService mapService)
    {
        _pathfinderService = pathfinderService ?? throw new ArgumentNullException(nameof(pathfinderService));
        _mapService = mapService ?? throw new ArgumentNullException(nameof(mapService));
    }

    public override void Configure()
    {
        Get("/api/maps/{id}/path");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Compute path from start to goal for a dungeon map";
            s.Description = "Retrieves a map and computes the optimal path from the stored start position to goal position, avoiding obstacles";
        });
    }

    public override async Task<Results<Ok<PathResponse>, ProblemHttpResult>> ExecuteAsync(GetMapByIdRequest req, CancellationToken ct)
    {
        try
        {
            req.Id = Route<Guid>("id");
            
            // Get the map with its built-in start and goal positions
            var map = await _mapService.GetMapByIdAsync(req.Id);
            if (map == null)
            {
                return TypedResults.Problem(
                    detail: $"Map with id {req.Id} not found",
                    statusCode: 404);
            }
            
            // Validate map has required positions
            if (map.StartPosition == null || map.GoalPosition == null)
            {
                return TypedResults.Problem(
                    detail: "Map is missing start or goal position",
                    statusCode: 400);
            }

            // Use the stored start and goal positions
            var result = await _pathfinderService.FindPathAsync(
                map, 
                map.StartPosition, 
                map.GoalPosition);

            var pathPoints = result.Path?.Select(p => new PointModel { X = p.X, Y = p.Y })?.ToList() ?? new List<PointModel>();
            
            // Create and validate response
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
                detail: $"Invalid request: {ex.Message}",
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