using Moq;
using TripApp.Application.Services;
using TripApp.Application.Repository;
using TripApp.Application.DTOs;
using TripApp.Core.Model;
using TripApp.Application.Exceptions;

namespace TripApp.Tests;

public class TripServiceTests
{
    private readonly Mock<ITripRepository> _mockTripRepository;
    private readonly TripService _tripService;

    public TripServiceTests()
    {
        _mockTripRepository = new Mock<ITripRepository>();
        _tripService = new TripService(_mockTripRepository.Object);
    }

    [Fact]
    public async Task AssignClientToTripAsync_SuccessfulAssignment_ShouldCallSaveChanges()
    {
        // Arrange
        var tripId = 1;
        var assignDto = new AssignClientToTripDto
        {
            IdTrip = tripId,
            TripName = "Test Trip",
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Telephone = "123-456-7890"
        };

        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "Test Trip", DateFrom = DateTime.UtcNow.AddDays(1) };
        var newClientFromRepo = new Client { IdClient = 100, Pesel = assignDto.Pesel };

        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);
        _mockTripRepository.Setup(r => r.ClientExistsWithPeselAsync(assignDto.Pesel)).ReturnsAsync(false);
        _mockTripRepository.Setup(r => r.IsClientRegisteredForTripAsync(tripId, assignDto.Pesel)).ReturnsAsync(false);
        _mockTripRepository.Setup(r => r.AddClientAsync(It.IsAny<Client>()))
            .ReturnsAsync(newClientFromRepo);
        _mockTripRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None);

        // Assert
        _mockTripRepository.Verify(r => r.GetTripByIdAsync(tripId), Times.Once);
        _mockTripRepository.Verify(r => r.ClientExistsWithPeselAsync(assignDto.Pesel), Times.Once);
        _mockTripRepository.Verify(r => r.IsClientRegisteredForTripAsync(tripId, assignDto.Pesel), Times.Once);
        _mockTripRepository.Verify(r => r.AddClientAsync(It.Is<Client>(c => c.Pesel == assignDto.Pesel)), Times.Once);
        _mockTripRepository.Verify(
            r => r.AddClientTripAsync(It.Is<ClientTrip>(ct =>
                ct.IdTrip == tripId && ct.IdClient == newClientFromRepo.IdClient)), Times.Once);
        _mockTripRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AssignClientToTripAsync_TripNotFound_ShouldThrowTripNotFoundException()
    {
        // Arrange
        var tripId = 1;
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Test Trip", Pesel = "123" };
        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync((TripApp.Core.Model.Trip)null);

        // Act & Assert
        await Assert.ThrowsAsync<TripExceptions.TripNotFoundException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));
    }

    [Fact]
    public async Task AssignClientToTripAsync_TripNameMismatch_ShouldThrowTripNameMismatchException()
    {
        // Arrange
        var tripId = 1;
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Wrong Name", Pesel = "123" };
        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "Correct Name", DateFrom = DateTime.UtcNow.AddDays(1) };
        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);

        // Act & Assert
        await Assert.ThrowsAsync<TripExceptions.TripNameMismatchException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));
    }

    [Fact]
    public async Task AssignClientToTripAsync_TripDateInPast_ShouldThrowTripDateInPastException()
    {
        // Arrange
        var tripId = 1;
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Past Trip", Pesel = "12345" };
        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "Past Trip", DateFrom = DateTime.UtcNow.AddDays(-1) };
        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);

        // Act & Assert
        await Assert.ThrowsAsync<TripExceptions.TripDateInPastException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));
    }


    [Fact]
    public async Task AssignClientToTripAsync_ClientWithPeselExists_ShouldThrowClientWithPeselExistsException()
    {
        // Arrange
        var tripId = 1;
        var pesel = "12345678901";
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Test Trip", Pesel = pesel };
        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "Test Trip", DateFrom = DateTime.UtcNow.AddDays(1) };
        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);
        _mockTripRepository.Setup(r => r.ClientExistsWithPeselAsync(pesel))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ClientExceptions.ClientWithPeselExistsException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));
    }

    [Fact]
    public async Task
        AssignClientToTripAsync_ClientAlreadyRegisteredForTrip_ShouldThrowClientAlreadyRegisteredForTripException()
    {
        // Arrange
        var tripId = 1;
        var pesel = "12345678901";
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Test Trip", Pesel = pesel };
        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "Test Trip", DateFrom = DateTime.UtcNow.AddDays(1) };

        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);
        _mockTripRepository.Setup(r => r.ClientExistsWithPeselAsync(pesel))
            .ReturnsAsync(
                false);
        _mockTripRepository.Setup(r => r.IsClientRegisteredForTripAsync(tripId, pesel))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ClientExceptions.ClientAlreadyRegisteredForTripException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));
    }

    [Fact]
    public async Task AssignClientToTripAsync_IdMismatchInRouteAndBody_ShouldThrowArgumentException()
    {
        // Arrange
        var tripIdInRoute = 1;
        var tripIdInBody = 2; // Mismatch
        var assignDto = new AssignClientToTripDto { IdTrip = tripIdInBody, TripName = "Test Trip", Pesel = "123" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _tripService.AssignClientToTripAsync(tripIdInRoute, assignDto, CancellationToken.None));
        Assert.Contains("Trip ID in the route does not match Trip ID in the request body", exception.Message);
    }
}