using TripApp.Core.Models;

namespace TripApp.Application.Repository;

public interface ITripRepository
{
    Task<PaginatedResult<Core.Models.Trip>> GetPaginatedTripsAsync(int page = 1, int pageSize = 10);
    Task<List<Core.Models.Trip>> GetAllTripsAsync();
}