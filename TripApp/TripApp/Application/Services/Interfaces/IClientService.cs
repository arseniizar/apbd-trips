namespace TripApp.Application.Services.Interfaces;

public interface IClientService
{
    Task<bool> ClientHasTripsAsync(int idClient);
    Task<bool> DeleteClientAsync(int idClient);
}