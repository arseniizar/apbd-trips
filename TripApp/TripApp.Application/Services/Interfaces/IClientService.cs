namespace TripApp.Application.Services.Interfaces;

public interface IClientService
{
    Task<bool> DeleteClientAsync(int idClient);
}