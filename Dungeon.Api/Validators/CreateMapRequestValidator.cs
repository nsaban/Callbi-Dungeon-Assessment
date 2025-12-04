using Dungeon.Api.Models;
using FastEndpoints;
using FluentValidation;

namespace Dungeon.Api.Validators;

public class CreateMapRequestValidator : Validator<CreateMapRequest>
{
    public CreateMapRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Map name is required")
            .MaximumLength(200).WithMessage("Map name cannot exceed 200 characters");

        RuleFor(x => x.Width)
            .GreaterThanOrEqualTo(5).WithMessage("Width must be greater than or equal to 5")
            .LessThanOrEqualTo(50).WithMessage("Width cannot exceed 50");

        RuleFor(x => x.Height)
            .GreaterThanOrEqualTo(5).WithMessage("Height must be greater than or equal to 5")
            .LessThanOrEqualTo(50).WithMessage("Height cannot exceed 50");

        RuleFor(x => x.StartPosition)
            .NotNull().WithMessage("Start position is required")
            .SetValidator(new PointModelValidator());

        RuleFor(x => x.GoalPosition)
            .NotNull().WithMessage("Goal position is required")
            .SetValidator(new PointModelValidator());

        RuleFor(x => x.Obstacles)
            .NotNull().WithMessage("Obstacles list cannot be null");

        RuleForEach(x => x.Obstacles)
            .SetValidator(new PointModelValidator());

        RuleFor(x => x.StartPosition.X)
            .GreaterThanOrEqualTo(0).WithMessage("Start position X coordinate must be non-negative")
            .LessThan(req => req.Width).WithMessage("Start position X coordinate must be less than map width");

        RuleFor(x => x.StartPosition.Y)
            .GreaterThanOrEqualTo(0).WithMessage("Start position Y coordinate must be non-negative")
            .LessThan(req => req.Height).WithMessage("Start position Y coordinate must be less than map height");

        RuleFor(x => x.GoalPosition.X)
            .GreaterThanOrEqualTo(0).WithMessage("Goal position X coordinate must be non-negative")
            .LessThan(req => req.Width).WithMessage("Goal position X coordinate must be less than map width");

        RuleFor(x => x.GoalPosition.Y)
            .GreaterThanOrEqualTo(0).WithMessage("Goal position Y coordinate must be non-negative")
            .LessThan(req => req.Height).WithMessage("Goal position Y coordinate must be less than map height");

        RuleFor(x => x)
            .Must(ValidateObstaclesWithinBounds)
            .WithMessage("All obstacles must be within map bounds")
            .Must(ValidateStartPositionNotOnObstacle)
            .WithMessage("Start position cannot be on an obstacle")
            .Must(ValidateGoalPositionNotOnObstacle)
            .WithMessage("Goal position cannot be on an obstacle");
    }

    private bool ValidateObstaclesWithinBounds(CreateMapRequest request)
    {
        return request.Obstacles.All(obstacle => 
            obstacle.X >= 0 && obstacle.X < request.Width &&
            obstacle.Y >= 0 && obstacle.Y < request.Height);
    }

    private bool ValidateStartPositionNotOnObstacle(CreateMapRequest request)
    {
        return !request.Obstacles.Any(obstacle =>
            obstacle.X == request.StartPosition.X &&
            obstacle.Y == request.StartPosition.Y);
    }

    private bool ValidateGoalPositionNotOnObstacle(CreateMapRequest request)
    {
        return !request.Obstacles.Any(obstacle =>
            obstacle.X == request.GoalPosition.X &&
            obstacle.Y == request.GoalPosition.Y);
    }
}