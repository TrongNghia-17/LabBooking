using LabBooking.Application.Features.Users.Dtos;

namespace LabBooking.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(
    IUserRepository userRepository,
    IMapper mapper
    ) : IRequestHandler<GetAllUsersQuery, PagedResult<UserResponse>>
{
    public async Task<PagedResult<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await userRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            request.RoleName
        );

        var userResponses = mapper.Map<List<UserResponse>>(users);

        var pagedResult = new PagedResult<UserResponse>(
            userResponses,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return pagedResult;
    }
}
