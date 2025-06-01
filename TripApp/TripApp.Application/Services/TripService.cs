using TripApp.Application.DTOs;
using TripApp.Application.Exceptions;
using TripApp.Application.Mappers;
using TripApp.Application.Repository;
using TripApp.Application.Services.Interfaces;
using TripApp.Core.Model;

namespace TripApp.Application.Services;

public class TripService(ITripRepository tripRepository) : ITripService
{
    public async Task<PaginatedResult<GetTripDto>> GetPaginatedTripsAsync(int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 10) pageSize = 10;
        var result = await tripRepository.GetPaginatedTripsAsync(page, pageSize);

        var mappedTrips = new PaginatedResult<GetTripDto>
        {
            AllPages = result.AllPages,
            Data = result.Data.Select(trip => trip.MapToGetTripDto()).ToList(),
            PageNum = result.PageNum,
            PageSize = result.PageSize
        };

        return mappedTrips;
    }

    public async Task<IEnumerable<GetTripDto>> GetAllTripsAsync()
    {
        var trips = await tripRepository.GetAllTripsAsync();
        var mappedTrips = trips.Select(trip => trip.MapToGetTripDto()).ToList();
        return mappedTrips;
    }

    public async Task AssignClientToTripAsync(int tripIdFromRoute, AssignClientToTripDto clientData,
        CancellationToken cancellationToken = default)
    {
        if (tripIdFromRoute != clientData.IdTrip)
        {
            throw new ArgumentException("Trip ID in the route does not match Trip ID in the request body.");
        }

        var trip = await tripRepository.GetTripByIdAsync(clientData.IdTrip);
        if (trip == null)
        {
            throw new TripExceptions.TripNotFoundException(clientData.IdTrip);
        }

        if (trip.Name != clientData.TripName)
        {
            throw new TripExceptions.TripNameMismatchException(clientData.TripName, clientData.IdTrip);
        }

        if (trip.DateFrom <= DateTime.UtcNow)
        {
            throw new TripExceptions.TripDateInPastException();
        }

        bool clientWithPeselExists = await tripRepository.ClientExistsWithPeselAsync(clientData.Pesel);
        if (clientWithPeselExists)
        {
            throw new ClientExceptions.ClientWithPeselExistsException(clientData.Pesel);
        }

        bool alreadyRegisteredForThisTrip =
            await tripRepository.IsClientRegisteredForTripAsync(clientData.IdTrip, clientData.Pesel);
        if (alreadyRegisteredForThisTrip)
        {
            throw new ClientExceptions.ClientAlreadyRegisteredForTripException(clientData.Pesel, clientData.IdTrip);
        }

        var newClient = new Client
        {
            FirstName = clientData.FirstName,
            LastName = clientData.LastName,
            Email = clientData.Email,
            Telephone = clientData.Telephone,
            Pesel = clientData.Pesel
        };

        var addedClient = await tripRepository.AddClientAsync(newClient);

        var clientTrip = new ClientTrip
        {
            IdClient = addedClient.IdClient,
            IdTrip = clientData.IdTrip,
            RegisteredAt = DateTime.UtcNow,
            PaymentDate = clientData.PaymentDate
        };

        await tripRepository.AddClientTripAsync(clientTrip);
        await tripRepository.SaveChangesAsync();
    }
}