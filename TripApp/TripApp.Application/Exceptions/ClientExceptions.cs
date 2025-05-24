namespace TripApp.Application.Exceptions;

public static class ClientExceptions
{
    public class ClientHasTripsException() 
        : InvalidOperationException("Client has trips.");
    
    public class ClientNotFoundException() 
        : BaseExceptions.NotFoundException("Client not found");
}