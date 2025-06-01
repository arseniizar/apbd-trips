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
        var tripId = 1;
        var assignDto = new AssignClientToTripDto
        {
            IdTrip = tripId,
            TripName = "Test Trip",
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Telephone = "123-456-7890",
            PaymentDate = DateTime.UtcNow.AddDays(-1)
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

        await _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None);

        _mockTripRepository.Verify(r => r.GetTripByIdAsync(tripId), Times.Once);
        _mockTripRepository.Verify(r => r.ClientExistsWithPeselAsync(assignDto.Pesel), Times.Once);
        _mockTripRepository.Verify(r => r.IsClientRegisteredForTripAsync(tripId, assignDto.Pesel), Times.Once);
        _mockTripRepository.Verify(r => r.AddClientAsync(It.Is<Client>(c => c.Pesel == assignDto.Pesel)), Times.Once);
        _mockTripRepository.Verify(
            r => r.AddClientTripAsync(It.Is<ClientTrip>(ct =>
                ct.IdTrip == tripId &&
                ct.IdClient == newClientFromRepo.IdClient &&
                ct.PaymentDate == assignDto.PaymentDate &&
                ct.RegisteredAt <= DateTime.UtcNow && ct.RegisteredAt > DateTime.UtcNow.AddSeconds(-5)
            )), Times.Once);
        _mockTripRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AssignClientToTripAsync_TripNotFound_ShouldThrowTripNotFoundException()
    {
        var tripId = 1;
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Test Trip", Pesel = "123" };
        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync((TripApp.Core.Model.Trip)null);

        await Assert.ThrowsAsync<TripExceptions.TripNotFoundException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));
    }

    [Fact]
    public async Task AssignClientToTripAsync_TripNameMismatch_ShouldThrowTripNameMismatchException()
    {
        var tripId = 1;
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Wrong Name", Pesel = "123" };
        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "Correct Name", DateFrom = DateTime.UtcNow.AddDays(1) };
        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);

        await Assert.ThrowsAsync<TripExceptions.TripNameMismatchException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));
    }

    [Fact]
    public async Task AssignClientToTripAsync_TripDateInPast_ShouldThrowTripDateInPastException()
    {
        var tripId = 1;
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Past Trip", Pesel = "12345" };
        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "Past Trip", DateFrom = DateTime.UtcNow.AddDays(-1) };
        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);

        await Assert.ThrowsAsync<TripExceptions.TripDateInPastException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));
    }


    [Fact]
    public async Task AssignClientToTripAsync_ClientWithPeselExists_ShouldThrowClientWithPeselExistsException()
    {
        var tripId = 1;
        var pesel = "12345678901";
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Test Trip", Pesel = pesel };
        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "Test Trip", DateFrom = DateTime.UtcNow.AddDays(1) };
        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);
        _mockTripRepository.Setup(r => r.ClientExistsWithPeselAsync(pesel))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ClientExceptions.ClientWithPeselExistsException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));
        _mockTripRepository.Verify(r => r.IsClientRegisteredForTripAsync(It.IsAny<int>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task
        AssignClientToTripAsync_ClientAlreadyRegisteredForTrip_ShouldThrowClientAlreadyRegisteredForTripException()
    {
        var tripId = 1;
        var pesel = "12345678901";
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Test Trip", Pesel = pesel };
        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "Test Trip", DateFrom = DateTime.UtcNow.AddDays(1) };

        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);
        _mockTripRepository.Setup(r => r.ClientExistsWithPeselAsync(pesel))
            .ReturnsAsync(false);
        _mockTripRepository.Setup(r => r.IsClientRegisteredForTripAsync(tripId, pesel))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ClientExceptions.ClientAlreadyRegisteredForTripException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));
        _mockTripRepository.Verify(r => r.AddClientAsync(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public async Task AssignClientToTripAsync_IdMismatchInRouteAndBody_ShouldThrowArgumentException()
    {
        var tripIdInRoute = 1;
        var tripIdInBody = 2;
        var assignDto = new AssignClientToTripDto { IdTrip = tripIdInBody, TripName = "Test Trip", Pesel = "123" };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _tripService.AssignClientToTripAsync(tripIdInRoute, assignDto, CancellationToken.None));
        Assert.Contains("Trip ID in the route does not match Trip ID in the request body", exception.Message);
        _mockTripRepository.Verify(r => r.GetTripByIdAsync(It.IsAny<int>()), Times.Never);
    }


    [Fact]
    public async Task AssignClientToTripAsync_SuccessfulAssignment_WithNullPaymentDate_ShouldPassNullToClientTrip()
    {
        var tripId = 5;
        var assignDto = new AssignClientToTripDto
        {
            IdTrip = tripId,
            TripName = "No Payment Trip",
            Pesel = "55555555555",
            FirstName = "Cash",
            LastName = "Customer",
            Email = "cash@example.com",
            Telephone = "555-0000",
            PaymentDate = null
        };

        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "No Payment Trip", DateFrom = DateTime.UtcNow.AddDays(10) };
        var newClientFromRepo = new Client { IdClient = 105, Pesel = assignDto.Pesel };

        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);
        _mockTripRepository.Setup(r => r.ClientExistsWithPeselAsync(assignDto.Pesel)).ReturnsAsync(false);
        _mockTripRepository.Setup(r => r.IsClientRegisteredForTripAsync(tripId, assignDto.Pesel)).ReturnsAsync(false);
        _mockTripRepository.Setup(r => r.AddClientAsync(It.IsAny<Client>())).ReturnsAsync(newClientFromRepo);
        _mockTripRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None);

        _mockTripRepository.Verify(r => r.AddClientTripAsync(It.Is<ClientTrip>(ct => ct.PaymentDate == null)),
            Times.Once);
        _mockTripRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AssignClientToTripAsync_AddClientAsyncFails_ShouldNotCallAddClientTripOrSaveChanges()
    {
        var tripId = 6;
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Client Add Fail", Pesel = "666" };
        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "Client Add Fail", DateFrom = DateTime.UtcNow.AddDays(1) };

        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);
        _mockTripRepository.Setup(r => r.ClientExistsWithPeselAsync(assignDto.Pesel)).ReturnsAsync(false);
        _mockTripRepository.Setup(r => r.IsClientRegisteredForTripAsync(tripId, assignDto.Pesel)).ReturnsAsync(false);
        _mockTripRepository.Setup(r => r.AddClientAsync(It.IsAny<Client>()))
            .ThrowsAsync(new InvalidOperationException("Simulated DB error on client add"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));

        _mockTripRepository.Verify(r => r.AddClientTripAsync(It.IsAny<ClientTrip>()), Times.Never);
        _mockTripRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AssignClientToTripAsync_AddClientTripAsyncFails_ShouldNotCallSaveChangesIfClientWasNotNew()
    {
        var tripId = 7;
        var pesel = "777";
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "ClientTrip Add Fail", Pesel = pesel };
        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "ClientTrip Add Fail", DateFrom = DateTime.UtcNow.AddDays(1) };
        var existingClient = new Client { IdClient = 107, Pesel = pesel };


        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);
        _mockTripRepository.Setup(r => r.ClientExistsWithPeselAsync(pesel)).ReturnsAsync(false);
        _mockTripRepository.Setup(r => r.IsClientRegisteredForTripAsync(tripId, pesel)).ReturnsAsync(false);
        _mockTripRepository.Setup(r => r.AddClientAsync(It.IsAny<Client>())).ReturnsAsync(existingClient);
        _mockTripRepository.Setup(r => r.AddClientTripAsync(It.IsAny<ClientTrip>()))
            .ThrowsAsync(new InvalidOperationException("Simulated DB error on client_trip add"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));

        _mockTripRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }


    [Fact]
    public async Task AssignClientToTripAsync_CheckOrder_PeselExistsGloballyCheckedBeforePeselForTrip()
    {
        var tripId = 8;
        var pesel = "88888888888";
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Order Check", Pesel = pesel };
        var tripFromRepo = new TripApp.Core.Model.Trip
            { IdTrip = tripId, Name = "Order Check", DateFrom = DateTime.UtcNow.AddDays(1) };

        var sequence = new MockSequence();
        _mockTripRepository.InSequence(sequence).Setup(r => r.GetTripByIdAsync(tripId)).ReturnsAsync(tripFromRepo);
        _mockTripRepository.InSequence(sequence).Setup(r => r.ClientExistsWithPeselAsync(pesel)).ReturnsAsync(true);
        _mockTripRepository.InSequence(sequence).Setup(r => r.IsClientRegisteredForTripAsync(tripId, pesel))
            .ReturnsAsync(false);


        await Assert.ThrowsAsync<ClientExceptions.ClientWithPeselExistsException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, CancellationToken.None));

        _mockTripRepository.Verify(r => r.IsClientRegisteredForTripAsync(tripId, pesel), Times.Never);
    }

    [Fact]
    public async Task AssignClientToTripAsync_CancellationRequested_ShouldThrowOperationCanceledException()
    {
        var tripId = 9;
        var assignDto = new AssignClientToTripDto { IdTrip = tripId, TripName = "Cancellation Test", Pesel = "999" };
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _mockTripRepository.Setup(r => r.GetTripByIdAsync(tripId))
            .Returns(async () =>
            {
                await Task.Delay(100, cancellationTokenSource.Token);
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                return new TripApp.Core.Model.Trip
                    { IdTrip = tripId, Name = "Cancellation Test", DateFrom = DateTime.UtcNow.AddDays(1) };
            });

        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            _tripService.AssignClientToTripAsync(tripId, assignDto, cancellationTokenSource.Token));
    }
}