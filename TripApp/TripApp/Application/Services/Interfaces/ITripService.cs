using TripApp.Application.DTOs;
using TripApp.Core.Models;

namespace TripApp.Application.Services.Interfaces;

public interface ITripService
{
    Task<PaginatedResult<GetTripDto>> GetPaginatedTripsAsync(int page = 1, int pageSize = 10);
    Task<List<GetTripDto>> GetAllTripsAsync();
}