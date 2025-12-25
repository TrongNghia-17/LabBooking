namespace LabBooking.Domain.Exceptions;

public class ForbiddenAccessException(string message) : Exception(message)
{
}
