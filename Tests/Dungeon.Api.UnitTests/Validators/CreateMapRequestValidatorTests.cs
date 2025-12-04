using Dungeon.Api.Models;
using Dungeon.Api.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Dungeon.Api.UnitTests.Validators;

[TestFixture]
public class CreateMapRequestValidatorTests
{
    private CreateMapRequestValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new CreateMapRequestValidator();
    }

    [Test]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        // Arrange
        var request = new CreateMapRequest { Name = "" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Map name is required");
    }

    [Test]
    public void Should_Have_Error_When_Name_Exceeds_MaxLength()
    {
        // Arrange
        var request = new CreateMapRequest { Name = new string('a', 201) };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Map name cannot exceed 200 characters");
    }

    [Test]
    public void Should_Have_Error_When_Width_Is_Too_Small()
    {
        // Arrange
        var request = new CreateMapRequest { Width = 4 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Width)
            .WithErrorMessage("Width must be greater than or equal to 5");
    }

    [Test]
    public void Should_Have_Error_When_Width_Is_Too_Large()
    {
        // Arrange
        var request = new CreateMapRequest { Width = 51 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Width)
            .WithErrorMessage("Width cannot exceed 50");
    }

    [Test]
    public void Should_Have_Error_When_Height_Is_Too_Small()
    {
        // Arrange
        var request = new CreateMapRequest { Height = 4 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Height)
            .WithErrorMessage("Height must be greater than or equal to 5");
    }

    [Test]
    public void Should_Have_Error_When_Height_Is_Too_Large()
    {
        // Arrange
        var request = new CreateMapRequest { Height = 51 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Height)
            .WithErrorMessage("Height cannot exceed 50");
    }

    [Test]
    public void Should_Have_Error_When_StartPosition_X_Is_Negative()
    {
        // Arrange
        var request = new CreateMapRequest
        {
            Width = 10,
            Height = 10,
            StartPosition = new PointModel { X = -1, Y = 0 }
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("StartPosition.X")
            .WithErrorMessage("Start position X coordinate must be non-negative");
    }

    [Test]
    public void Should_Have_Error_When_StartPosition_X_Exceeds_Width()
    {
        // Arrange
        var request = new CreateMapRequest
        {
            Width = 10,
            Height = 10,
            StartPosition = new PointModel { X = 10, Y = 0 }
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("StartPosition.X")
            .WithErrorMessage("Start position X coordinate must be less than map width");
    }

    [Test]
    public void Should_Have_Error_When_GoalPosition_Y_Exceeds_Height()
    {
        // Arrange
        var request = new CreateMapRequest
        {
            Width = 10,
            Height = 8,
            GoalPosition = new PointModel { X = 5, Y = 8 }
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("GoalPosition.Y")
            .WithErrorMessage("Goal position Y coordinate must be less than map height");
    }

    [Test]
    public void Should_Have_Error_When_StartPosition_Is_On_Obstacle()
    {
        // Arrange
        var request = new CreateMapRequest
        {
            Name = "Test Map",
            Width = 10,
            Height = 10,
            StartPosition = new PointModel { X = 2, Y = 3 },
            GoalPosition = new PointModel { X = 8, Y = 7 },
            Obstacles = new List<PointModel>
            {
                new() { X = 2, Y = 3 }, // Same as start position
                new() { X = 5, Y = 6 }
            }
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Start position cannot be on an obstacle");
    }

    [Test]
    public void Should_Have_Error_When_GoalPosition_Is_On_Obstacle()
    {
        // Arrange
        var request = new CreateMapRequest
        {
            Name = "Test Map",
            Width = 10,
            Height = 10,
            StartPosition = new PointModel { X = 1, Y = 1 },
            GoalPosition = new PointModel { X = 5, Y = 6 },
            Obstacles = new List<PointModel>
            {
                new() { X = 2, Y = 3 },
                new() { X = 5, Y = 6 } // Same as goal position
            }
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Goal position cannot be on an obstacle");
    }

    [Test]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var request = new CreateMapRequest
        {
            Name = "Valid Test Map",
            Width = 10,
            Height = 8,
            StartPosition = new PointModel { X = 0, Y = 0 },
            GoalPosition = new PointModel { X = 9, Y = 7 },
            Obstacles = new List<PointModel>
            {
                new() { X = 2, Y = 3 },
                new() { X = 5, Y = 6 }
            }
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}