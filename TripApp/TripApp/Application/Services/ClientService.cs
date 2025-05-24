using TripApp.Application.Repository;
using TripApp.Application.Services.Interfaces;

namespace TripApp.Application.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    public async Task<bool> ClientHasTripsAsync(int idClient)
    {
        var clientExists = await clientRepository.ClientExistsAsync(idClient);
        if (!clientExists)
            throw new Exception("Client does not exist");
        
        return await clientRepository.ClientHasTripsAsync(idClient);
    }

    public async Task<bool> DeleteClientAsync(int idClient)
    {
        var clientExists = await clientRepository.ClientExistsAsync(idClient);
        if (!clientExists)
            throw new Exception("Client does not exist");
        
        var clientHasTrips = await ClientHasTripsAsync(idClient);
        if (clientHasTrips)
            throw new Exception("Client has trips");
        
        return await clientRepository.DeleteClientAsync(idClient);
    }
}