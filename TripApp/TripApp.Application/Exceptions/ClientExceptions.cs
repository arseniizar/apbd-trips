namespace TripApp.Application.Exceptions;

public static class ClientExceptions
{
    public class ClientHasTripsException()
        : InvalidOperationException("Client has trips.");

    public class ClientNotFoundException(string clientId)
        : BaseExceptions.NotFoundException($"Client not found with client id {clientId}");

    public class ClientWithPeselExistsException(string pesel)
        : InvalidOperationException($"A client with PESEL '{pesel}' already exists in the system.");

    public class ClientAlreadyRegisteredForTripException(string pesel, int tripId)
        : InvalidOperationException($"Client with PESEL '{pesel}' is already registered for trip ID {tripId}.");
}