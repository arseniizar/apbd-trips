using TripApp.Core.Model;

namespace TripApp.Application.Repository;

public interface ITripRepository
{
    Task<PaginatedResult<Trip>> GetPaginatedTripsAsync(int page = 1, int pageSize = 10);
    Task<List<Trip>> GetAllTripsAsync();
    Task<Trip?> GetTripByIdAsync(int tripId);
    Task<bool> ClientExistsWithPeselAsync(string pesel);
    Task<bool> IsClientRegisteredForTripAsync(int tripId, string pesel);
    Task<Client> AddClientAsync(Client client);
    Task AddClientTripAsync(ClientTrip clientTrip);
    Task<int> SaveChangesAsync();
}