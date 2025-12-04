using Dungeon.Api.Models;
using Dungeon.Application.Interfaces;
using FastEndpoints;
using Dungeon.Api.Validators;

namespace Dungeon.Api.Endpoints.Maps;

public class GetMapByIdEndpoint : Endpoint<GetMapByIdRequest, MapResponse>
{
    private readonly IMapService _mapService;

    public GetMapByIdEndpoint(IMapService mapService)
    {
        _mapService = mapService ?? throw new ArgumentNullException(nameof(mapService));
    }

    public override void Configure()
    {
        Get("/api/maps/{id}");
        AllowAnonymous();
        Validator<GetMapByIdRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Get a dungeon map by ID";
            s.Description = "Retrieves a specific dungeon map by its unique identifier";
        });
    }

    public override async Task HandleAsync(GetMapByIdRequest req, CancellationToken ct)
    {
        req.Id = Route<Guid>("id");
        var map = await _mapService.GetMapByIdAsync(req.Id);
        
        if (map == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = new MapResponse
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