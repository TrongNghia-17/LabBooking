namespace LabBooking.Application.Features.Supports.Commands.DeleteSupport;

/// <summary>
/// Represents the command to delete a support ticket.
/// </summary>
/// <param name="Id">The unique identifier of the support ticket to delete.</param>
/// <remarks>
/// This command uses IRequest&lt;Unit&gt; because no specific data needs to be returned
/// upon successful deletion, only an acknowledgment that the operation is complete.
/// </remarks>
public record DeleteSupportCommand(Guid Id) : IRequest<Unit>;
