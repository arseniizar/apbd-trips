using TripApp.Core.Model;

namespace TripApp.Application.Repository;

public interface ITripRepository
{
    Task<PaginatedResult<Core.Model.Trip>> GetPaginatedTripsAsync(int page = 1, int pageSize = 10);
    Task<List<Core.Model.Trip>> GetAllTripsAsync();
}