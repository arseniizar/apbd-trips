using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Trip.API.Controllers;
using TripApp.Application.Services.Interfaces;
using TripApp.Application.DTOs;
using TripApp.Application.Exceptions;

namespace TripApp.Tests;

public class TripControllerTests
{
    private readonly Mock<ITripService> _mockTripService;
    private readonly TripController _tripController;

    public TripControllerTests()
    {
        _mockTripService = new Mock<ITripService>();
        _tripController = new TripController(_mockTripService.Object);
    }

    [Fact]
    public async Task AssignClientToTrip_Successful_Returns201Created()
    {
        // Arrange
        var tripId = 1;
        var assignDto = new AssignClientToTripDto
        {
            IdTrip = tripId,
            FirstName = "John",
            LastName = "Doe",
            Email = "j.doe@example.com",
            Telephone = "123456789",
            Pesel = "12345678901",
            TripName = "Amazing Trip",
            PaymentDate = null
        };
        _mockTripService.Setup(s => s.AssignClientToTripAsync(tripId, assignDto, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tripController.AssignClientToTrip(tripId, assignDto, CancellationToken.None);

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status201Created, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task AssignClientToTrip_TripNotFound_Returns404NotFound()
    {
        // Arrange
        var tripId = 1;
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Lost Trip" };
        _mockTripService.Setup(s => s.AssignClientToTripAsync(tripId, assignDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TripExceptions.TripNotFoundException(tripId));

        // Act
        var result = await _tripController.AssignClientToTrip(tripId, assignDto, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.Contains($"Trip with ID {tripId} not found.", notFoundResult.Value.ToString());
    }

    [Fact]
    public async Task AssignClientToTrip_ClientWithPeselExists_Returns409Conflict()
    {
        // Arrange
        var tripId = 1;
        var pesel = "12345";
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, Pesel = pesel, TripName = "Existing Pesel Trip" };
        _mockTripService.Setup(s => s.AssignClientToTripAsync(tripId, assignDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ClientExceptions.ClientWithPeselExistsException(pesel));

        // Act
        var result = await _tripController.AssignClientToTrip(tripId, assignDto, CancellationToken.None);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
        Assert.Contains($"A client with PESEL '{pesel}' already exists", conflictResult.Value.ToString());
    }

    [Fact]
    public async Task AssignClientToTrip_ClientAlreadyRegisteredForTrip_Returns409Conflict()
    {
        // Arrange
        var tripId = 2;
        var pesel = "98765";
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, Pesel = pesel, TripName = "Re-registration Trip" };
        _mockTripService.Setup(s => s.AssignClientToTripAsync(tripId, assignDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ClientExceptions.ClientAlreadyRegisteredForTripException(pesel, tripId));

        // Act
        var result = await _tripController.AssignClientToTrip(tripId, assignDto, CancellationToken.None);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
        Assert.Contains($"Client with PESEL '{pesel}' is already registered for trip ID {tripId}",
            conflictResult.Value.ToString());
    }

    [Fact]
    public async Task AssignClientToTrip_TripDateInPast_Returns400BadRequest()
    {
        // Arrange
        var tripId = 3;
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Past Adventure" };
        _mockTripService.Setup(s => s.AssignClientToTripAsync(tripId, assignDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TripExceptions.TripDateInPastException());

        // Act
        var result = await _tripController.AssignClientToTrip(tripId, assignDto, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Contains("Cannot register for a trip that has already started or occurred.",
            badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task AssignClientToTrip_TripNameMismatch_Returns400BadRequest()
    {
        // Arrange
        var tripId = 4;
        var providedName = "Wrong Trip Name";
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = providedName };
        _mockTripService.Setup(s => s.AssignClientToTripAsync(tripId, assignDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TripExceptions.TripNameMismatchException(providedName, tripId));

        // Act
        var result = await _tripController.AssignClientToTrip(tripId, assignDto, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Contains($"The provided trip name '{providedName}' does not match the name of trip ID {tripId}",
            badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task AssignClientToTrip_ArgumentExceptionFromService_Returns400BadRequest()
    {
        // Arrange
        var tripId = 5;
        var assignDto = new AssignClientToTripDto
            { IdTrip = 6, TripName = "ID Mismatch Test" };
        var errorMessage =
            "Trip ID in the route does not match Trip ID in the request body.";

        _mockTripService.Setup(s => s.AssignClientToTripAsync(tripId, assignDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _tripController.AssignClientToTrip(tripId, assignDto, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Equal(errorMessage, badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task AssignClientToTrip_UnhandledInvalidOperationException_Returns400BadRequest()
    {
        // Arrange
        var tripId = 7;
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Generic Invalid Op" };
        var errorMessage = "Some generic business rule was violated.";
        _mockTripService.Setup(s => s.AssignClientToTripAsync(tripId, assignDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(errorMessage));

        // Act
        var result = await _tripController.AssignClientToTrip(tripId, assignDto, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Equal(errorMessage, badRequestResult.Value.ToString());
    }
}