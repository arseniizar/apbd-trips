using TripApp.Core.Model;

namespace TripApp.Core.Repository;

public interface ITripRepository
{
    Task<PaginatedResult<Model.Trip>> GetPaginatedTripsAsync(int page = 1, int pageSize = 10);
    Task<List<Model.Trip>> GetAllTripsAsync();
}