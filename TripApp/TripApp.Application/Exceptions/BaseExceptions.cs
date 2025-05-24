namespace TripApp.Application.Exceptions;

public static class BaseExceptions
{
    public class NotFoundException(string message) : Exception(message);
    public class ValidationException(string message) : Exception(message);
}