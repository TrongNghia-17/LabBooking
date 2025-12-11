using System.Linq.Expressions;

namespace LabBooking.Application.Interfaces.Infrastructure;

public interface IBackgroundJobService
{
    void Enqueue(Expression<Action> methodCall);
}
