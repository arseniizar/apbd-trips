using Microsoft.EntityFrameworkCore;
using TripApp.Application.Repository;
using TripApp.Core.Models;

namespace TripApp.Infrastructure.Repository;

public class TripRepository(TripContext tripDbContext) : ITripRepository
{
    public async Task<PaginatedResult<Core.Models.Trip>> GetPaginatedTripsAsync(int page = 1, int pageSize = 10)
    {
        var tripsQuery = tripDbContext.Trips
            .Include(e => e.ClientTrips).ThenInclude(e => e.IdClientNavigation)
            .Include(e => e.IdCountries)
            .OrderByDescending(e => e.DateFrom);

        var tripsCount = await tripsQuery.CountAsync();
        var totalPages = tripsCount / pageSize;
        var trips = await tripsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Core.Models.Trip>
        {
            PageSize = pageSize,
            PageNum = page,
            AllPages = totalPages,
            Data = trips
        };
    }

    public async Task<List<Core.Models.Trip>> GetAllTripsAsync()
    {
        return await tripDbContext.Trips
            .Include(e => e.ClientTrips).ThenInclude(e => e.IdClientNavigation)
            .Include(e => e.IdCountries)
            .OrderBy(e => e.DateFrom)
            .ToListAsync();
    }
}