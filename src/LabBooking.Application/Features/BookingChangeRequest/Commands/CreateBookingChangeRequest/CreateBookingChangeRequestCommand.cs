using LabBooking.Application.Features.BookingChangeRequest.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Commands.CreateBookingChangeRequest
{
    public record CreateBookingChangeRequestCommand(
        Guid BookingId,
        Guid RequestedById,
        // Các trường đơn giản
        string? NewTitle,
        string? NewDescription,
        int? NewNumberOfParticipants,
        Guid? NewCourseId,
        // Các object con (Frontend gửi Object, Backend tự parse sang JSON)
        ChangeProjectInput? NewProject,
        ChangePriorityInput? NewPriorityDetail,
        List<ChangeEquipmentInput>? NewExternalEquipments,
        List<ChangeOutSideGuestInput>? NewOutSideGuests,
        // Danh sách slot mong muốn
        List<ChangeSlotInput> DesiredSlots
    ) : IRequest<BookingChangeRequestResponse>; 

    public record ChangeProjectInput(string ProjectName, string? Description, ProjectType ProjectType);
    public record ChangePriorityInput(string? Justification, string? EvidenceFilePath);
    public record ChangeEquipmentInput(string Name, string? Description, int Quantity);
    public record ChangeSlotInput(DateOnly Date, Guid SlotId);
    public record ChangeOutSideGuestInput(string FullName, string Email, string? Organization, string? PurposeOfVisit);
}
