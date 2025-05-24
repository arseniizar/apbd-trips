using TripApp.Application.Exceptions;
using TripApp.Application.Repository;
using TripApp.Application.Services.Interfaces;

namespace TripApp.Application.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    public async Task<bool> DeleteClientAsync(int idClient)
    {
        var clientExists = await clientRepository.ClientExistsAsync(idClient);
        if (!clientExists) 
            return false;
        
        var clientHasTrips = await clientRepository.ClientHasTripsAsync(idClient);
        if (clientHasTrips)
            throw new ClientExceptions.ClientHasTripsException();
        
        return await clientRepository.DeleteClientAsync(idClient);
    }
}