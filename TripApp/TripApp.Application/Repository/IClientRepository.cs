namespace TripApp.Application.Repository;

public interface IClientRepository
{ 
    Task<bool> ClientExistsAsync(int idClient);
    Task<bool> ClientHasTripsAsync(int idClient);
    Task<bool> DeleteClientAsync(int idClient);
}