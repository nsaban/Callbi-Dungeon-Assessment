using Dungeon.Api.Models;
using Dungeon.Api.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Dungeon.Api.UnitTests.Validators;

[TestFixture]
public class GetMapByIdRequestValidatorTests
{
    private GetMapByIdRequestValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new GetMapByIdRequestValidator();
    }

    [Test]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var request = new GetMapByIdRequest { Id = Guid.Empty };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Map ID is required");
    }

    [Test]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var request = new GetMapByIdRequest { Id = Guid.NewGuid() };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}