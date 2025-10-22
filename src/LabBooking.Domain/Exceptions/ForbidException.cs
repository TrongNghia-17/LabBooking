namespace LabBooking.Domain.Exceptions;

public class ForbidException(string message) : Exception(message)
{
}
