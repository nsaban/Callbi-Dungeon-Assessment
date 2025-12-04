using Dungeon.Api.Models;
using Dungeon.Application.Interfaces;
using FastEndpoints;

namespace Dungeon.Api.Endpoints.Maps;

public class GetAllMapsEndpoint : EndpointWithoutRequest<List<MapResponse>>
{
    private readonly IMapService _mapService;

    public GetAllMapsEndpoint(IMapService mapService)
    {
        _mapService = mapService ?? throw new ArgumentNullException(nameof(mapService));
    }

    public override void Configure()
    {
        Get("/api/maps");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get all dungeon maps";
            s.Description = "Retrieves all dungeon maps in the system";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var maps = await _mapService.GetAllMapsAsync();
        
        var response = maps.Select(map => new MapResponse
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

        await SendOkAsync(response, ct);
    }
}