using Dungeon.Api.Models;
using FastEndpoints;
using FluentValidation;

namespace Dungeon.Api.Validators;

public class PointModelValidator : Validator<PointModel>
{
    public PointModelValidator()
    {
        RuleFor(x => x.X)
            .GreaterThanOrEqualTo(0).WithMessage("X coordinate must be non-negative");

        RuleFor(x => x.Y)
            .GreaterThanOrEqualTo(0).WithMessage("Y coordinate must be non-negative");
    }
}