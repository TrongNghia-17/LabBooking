using LabBooking.Application.Features.Supports.Dtos;
using LabBooking.Application.Features.Supports.Queries.GetByIdSupport;
using LabBooking.Application.Services.Users; // Cần thêm dòng này

namespace LabBooking.Application.Features.Supports.Queries.GetMySupports;

// Đổi tên từ GetSupportByIdQueryHandler sang GetMySupportsQueryHandler
public class GetMySupportsQueryHandler(
    ILogger<GetMySupportsQueryHandler> logger,
    ISupportRepository supportRepository,
    ICurrentUserService currentUserService, // Thêm service lấy User ID
    IMapper mapper) : IRequestHandler<GetMySupportsQuery, IEnumerable<SupportsResponse>>
{
    public async Task<IEnumerable<SupportsResponse>> Handle(GetMySupportsQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy ID của người dùng đang đăng nhập
        var currentUserId = currentUserService.UserId;

        if (currentUserId == null)
        {
            logger.LogWarning("Authenticated user ID not found. Cannot retrieve support tickets.");
            throw new UnauthorizedAccessException("User must be authenticated to retrieve their support tickets.");
        }

        logger.LogInformation("Processing GetMySupportsQuery for UserId: {UserId}", currentUserId.Value);

        // 2. Gọi Repository để lấy tất cả ticket dựa trên CreatedById
        // Bạn cần phải tạo phương thức này trong ISupportRepository và SupportRepository.cs
        var supports = await supportRepository.GetByCreatedByIdAsync(currentUserId.Value, cancellationToken);

        if (supports == null) // Xử lý nếu repository trả về null (mặc dù nên trả về list rỗng)
        {
            return Enumerable.Empty<SupportsResponse>();
        }

        // 3. Map sang DTO và trả về
        var supportResponses = mapper.Map<IEnumerable<SupportsResponse>>(supports);

        logger.LogInformation("Successfully retrieved {Count} support tickets for user {UserId}", supportResponses.Count(), currentUserId.Value);

        return supportResponses;
    }
}