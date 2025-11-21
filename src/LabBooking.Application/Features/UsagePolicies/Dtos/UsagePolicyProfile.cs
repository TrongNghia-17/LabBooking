using LabBooking.Application.Features.UsagePolicies.Commands.CreateUsagePolicy;
using LabBooking.Application.Features.UsagePolicies.Commands.UpdateUsagePolicy;

namespace LabBooking.Application.Features.UsagePolicies.Dtos;

public class UsagePolicyProfile : Profile
{
    public UsagePolicyProfile()
    {
        CreateMap<CreateUsagePolicyCommand, UsagePolicy>();
        CreateMap<UpdateUsagePolicyCommand, UsagePolicy>();
        CreateMap<UsagePolicy, UsagePolicyResponse>();
    }
}
