namespace LabBooking.Domain.Repositories;

public interface IIncidentRepository
{
    Task<IEnumerable<Incident>> GetAllAsync();
}
