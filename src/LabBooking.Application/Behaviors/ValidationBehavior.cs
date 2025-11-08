namespace LabBooking.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra xem có validator nào cho TRequest này không
        if (!validators.Any())
        {
            return await next(cancellationToken); // Không có, cho đi tiếp
        }

        // 2. Tạo validation context
        var context = new ValidationContext<TRequest>(request);

        // 3. Chạy tất cả validator và gom lỗi (nếu có)
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .SelectMany(r => r.Errors) // Lấy tất cả lỗi từ các validator
            .Where(f => f != null)     // Lọc ra các lỗi hợp lệ
            .ToList();

        // 4. Nếu có lỗi, NÉM ra exception
        if (failures.Any())
        {
            // FluentValidation cung cấp sẵn exception này
            throw new ValidationException(failures);
        }

        // 5. Không có lỗi, cho request đi tiếp đến Handler
        return await next(cancellationToken);
    }
}
