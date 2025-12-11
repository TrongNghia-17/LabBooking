using Hangfire;
using LabBooking.Application.Services;

namespace LabBooking.Infrastructure.Services;

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
