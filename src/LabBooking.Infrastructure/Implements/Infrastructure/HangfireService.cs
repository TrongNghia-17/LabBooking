using Hangfire;
using LabBooking.Application.Interfaces.Infrastructure;

namespace LabBooking.Infrastructure.Implements.Infrastructure;

public class HangfireService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    public HangfireService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void Enqueue(Expression<Action> methodCall)
    {
        _backgroundJobClient.Enqueue(methodCall);
    }
}
