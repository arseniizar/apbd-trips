using Microsoft.EntityFrameworkCore;
using TripApp.Application.Repository;
using TripApp.Core.Model;

namespace Trip.Infrastructure.Repository;

public class TripRepository(TripContext tripDbContext) : ITripRepository
{
    /// <summary>
    /// Retrieves a paginated list of trips based on the specified page number and page size.
    /// </summary>
    /// <param name="page">The current page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items to include on each page. Defaults to 10.</param>
    /// <returns>A <see cref="PaginatedResult{Trip}"/> containing the paginated list of trips and metadata such as page size, page number, and total pages.</returns>
    public async Task<PaginatedResult<TripApp.Core.Model.Trip>> GetPaginatedTripsAsync(int page = 1, int pageSize = 10)
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

        return new PaginatedResult<TripApp.Core.Model.Trip>
        {
            PageSize = pageSize,
            PageNum = page,
            AllPages = totalPages,
            Data = trips
        };
    }

    public async Task<List<TripApp.Core.Model.Trip>> GetAllTripsAsync()
    {
        return await tripDbContext.Trips
            .Include(e => e.ClientTrips).ThenInclude(e => e.IdClientNavigation)
            .Include(e => e.IdCountries)
            .OrderBy(e => e.DateFrom)
            .ToListAsync();
    }

    public async Task<TripApp.Core.Model.Trip?> GetTripByIdAsync(int tripId)
    {
        return await tripDbContext.Trips
            .FirstOrDefaultAsync(t => t.IdTrip == tripId);
    }

    public async Task<bool> ClientExistsWithPeselAsync(string pesel)
    {
        return await tripDbContext.Clients
            .AnyAsync(c => c.Pesel == pesel);
    }

    public async Task<bool> IsClientRegisteredForTripAsync(int tripId, string pesel)
    {
        return await tripDbContext.ClientTrips
            .Include(ct => ct.IdClientNavigation)
            .AnyAsync(ct => ct.IdTrip == tripId && ct.IdClientNavigation.Pesel == pesel);
    }

    public async Task<Client> AddClientAsync(Client client)
    {
        await tripDbContext.Clients.AddAsync(client);
        return
            client; 
    }

    public async Task AddClientTripAsync(ClientTrip clientTrip)
    {
        await tripDbContext.ClientTrips.AddAsync(clientTrip);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await tripDbContext.SaveChangesAsync();
    }
}