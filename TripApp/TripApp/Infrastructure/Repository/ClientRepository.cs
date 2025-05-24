using Microsoft.EntityFrameworkCore;
using TripApp.Application.Repository;

namespace TripApp.Infrastructure.Repository;

public class ClientRepository(TripContext context) : IClientRepository
{
    public async Task<bool> ClientExistsAsync(int idClient)
    {
        var client = await context.Clients.FirstOrDefaultAsync(x => x.IdClient == idClient);
        return client is not null;
    }

    public async Task<bool> ClientHasTripsAsync(int idClient)
    {
        return await context.ClientTrips.AnyAsync(ct => ct.IdClient == idClient);
    }

    public async Task<bool> DeleteClientAsync(int idClient)
    {
        var client = await context.Clients.FindAsync(idClient);
        if (client == null)
        {
            return false;
        }

        context.Clients.Remove(client);
        await context.SaveChangesAsync();
        return true;
    }
}