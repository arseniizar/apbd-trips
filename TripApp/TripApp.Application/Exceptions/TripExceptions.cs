namespace TripApp.Application.Exceptions;

public static class TripExceptions
{
    public class TripNotFoundException(int tripId)
        : BaseExceptions.NotFoundException($"Trip with ID {tripId} not found.");

    public class TripDateInPastException()
        : InvalidOperationException("Cannot register for a trip that has already started or occurred.");

    public class TripNameMismatchException(string providedName, int tripId)
        : BaseExceptions.ValidationException(
            $"The provided trip name '{providedName}' does not match the name of trip ID {tripId}.");
}