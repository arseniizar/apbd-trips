using Microsoft.EntityFrameworkCore;
using TripApp.Application.Repository;
using TripApp.Core.Models;

namespace TripApp.Infrastructure.Repository;

public class TripRepository : ITripRepository
{
    private readonly TripdbContext _tripDbContext;

    public TripRepository(TripdbContext tripDbContext)
    {
        _tripDbContext = tripDbContext;
    }

    public async Task<PaginatedResult<Trip>> GetPaginatedTripsAsync(int page = 1, int pageSize = 10)
    {
        var tripsQuery = _tripDbContext.Trips
            .Include(e => e.ClientTrips).ThenInclude(e => e.IdClientNavigation)
            .Include(e => e.IdCountries)
            .OrderBy(e => e.DateFrom);

        var tripsCount = await tripsQuery.CountAsync();
        var totalPages = tripsCount / pageSize;
        var trips = await _tripDbContext.Trips
            .Include(e => e.ClientTrips).ThenInclude(e => e.IdClientNavigation)
            .Include(e => e.IdCountries)
            .OrderBy(e => e.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Trip>
        {
            PageSize = pageSize,
            PageNum = page,
            AllPages = totalPages,
            Data = trips
        };
    }

    public async Task<List<Trip>> GetAllTripsAsync()
    {
        return await _tripDbContext.Trips
            .Include(e => e.ClientTrips).ThenInclude(e => e.IdClientNavigation)
            .Include(e => e.IdCountries)
            .OrderBy(e => e.DateFrom)
            .ToListAsync();
    }
}