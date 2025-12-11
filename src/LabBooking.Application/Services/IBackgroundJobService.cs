using System.Linq.Expressions;

namespace LabBooking.Application.Services;

public interface IBackgroundJobService
{
    void Enqueue(Expression<Action> methodCall);
}
