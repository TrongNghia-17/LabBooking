using System.Linq.Expressions;

namespace LabBooking.Application.Interfaces.Infrastructure;

public interface IBackgroundJobService
{
    // Gửi ngay lập tức
    void Enqueue(Expression<Action> methodCall);

    // Gửi sau một khoảng thời gian (QUAN TRỌNG ĐỂ FIX LỖI DISCONNECT)
    void Schedule(Expression<Action> methodCall, TimeSpan delay);
}
