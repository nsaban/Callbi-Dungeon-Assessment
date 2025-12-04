using FastEndpoints;
using FluentValidation;
using Dungeon.Api.Models;

namespace Dungeon.Api.Validators;

public class GetMapByIdRequestValidator : Validator<GetMapByIdRequest>
{
    public GetMapByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Map ID is required");
    }
}