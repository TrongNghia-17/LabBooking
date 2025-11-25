using LabBooking.Application.Features.Supports.Commands.CreateSupport;
using LabBooking.Application.Features.Supports.Commands.UpdateSupport;

namespace LabBooking.Application.Features.Supports.Dtos;

/// <summary>
/// Configures AutoMapper profiles for the Support feature.
/// </summary>
/// <remarks>
/// This class defines the mapping configurations between the Support domain entity,
/// its corresponding DTOs (like <see cref="SupportsResponse"/>), and CQS command objects
/// (like <see cref="CreateSupportCommand"/> and <see cref="UpdateSupportCommand"/>).
/// </remarks>
public class SupportsProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SupportsProfile"/> class
    /// and defines the object-to-object mappings.
    /// </summary>
    public SupportsProfile()
    {
        CreateMap<Support, SupportsResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateSupportCommand, Support>()
            .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => string.Empty));

        CreateMap<UpdateSupportCommand, Support>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore());
    }
}
