using Dungeon.Api.Models;
using Dungeon.Api.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Dungeon.Api.UnitTests.Validators;

[TestFixture]
public class PointModelValidatorTests
{
    private PointModelValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new PointModelValidator();
    }

    [Test]
    public void Should_Have_Error_When_X_Is_Negative()
    {
        // Arrange
        var point = new PointModel { X = -1, Y = 5 };

        // Act & Assert
        var result = _validator.TestValidate(point);
        result.ShouldHaveValidationErrorFor(x => x.X)
            .WithErrorMessage("X coordinate must be non-negative");
    }

    [Test]
    public void Should_Have_Error_When_Y_Is_Negative()
    {
        // Arrange
        var point = new PointModel { X = 5, Y = -1 };

        // Act & Assert
        var result = _validator.TestValidate(point);
        result.ShouldHaveValidationErrorFor(x => x.Y)
            .WithErrorMessage("Y coordinate must be non-negative");
    }

    [Test]
    public void Should_Have_Errors_When_Both_Coordinates_Are_Negative()
    {
        // Arrange
        var point = new PointModel { X = -1, Y = -1 };

        // Act & Assert
        var result = _validator.TestValidate(point);
        result.ShouldHaveValidationErrorFor(x => x.X)
            .WithErrorMessage("X coordinate must be non-negative");
        result.ShouldHaveValidationErrorFor(x => x.Y)
            .WithErrorMessage("Y coordinate must be non-negative");
    }

    [Test]
    public void Should_Not_Have_Error_When_Coordinates_Are_Zero()
    {
        // Arrange
        var point = new PointModel { X = 0, Y = 0 };

        // Act & Assert
        var result = _validator.TestValidate(point);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Should_Not_Have_Error_When_Coordinates_Are_Positive()
    {
        // Arrange
        var point = new PointModel { X = 5, Y = 10 };

        // Act & Assert
        var result = _validator.TestValidate(point);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Should_Not_Have_Error_When_Coordinates_Are_Large_Values()
    {
        // Arrange
        var point = new PointModel { X = 1000, Y = 999 };

        // Act & Assert
        var result = _validator.TestValidate(point);
        result.ShouldNotHaveAnyValidationErrors();
    }
}