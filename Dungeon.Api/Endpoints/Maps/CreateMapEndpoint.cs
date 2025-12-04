using Dungeon.Api.Models;
using Dungeon.Application.Interfaces;
using FastEndpoints;
using Dungeon.Api.Validators;

namespace Dungeon.Api.Endpoints.Maps;

public class CreateMapEndpoint : Endpoint<CreateMapRequest, CreateMapResponse>
{
    private readonly IMapService _mapService;

    public CreateMapEndpoint(IMapService mapService)
    {
        _mapService = mapService ?? throw new ArgumentNullException(nameof(mapService));
    }

    public override void Configure()
    {
        Post("/api/maps");
        AllowAnonymous();
        Validator<CreateMapRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Create a new dungeon map";
            s.Description = "Creates a new dungeon map with specified dimensions and obstacles";
        });
    }

    public override async Task HandleAsync(CreateMapRequest req, CancellationToken ct)
    {
        var obstacles = req.Obstacles.Select(o => new Application.Models.Point(o.X, o.Y)).ToList();
        var startPosition = new Application.Models.Point(req.StartPosition.X, req.StartPosition.Y);
        var goalPosition = new Application.Models.Point(req.GoalPosition.X, req.GoalPosition.Y);
        var map = await _mapService.CreateMapAsync(req.Name, req.Width, req.Height, startPosition, goalPosition, obstacles);
        
        var response = new CreateMapResponse
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

        await SendOkAsync(response, ct);
    }
}