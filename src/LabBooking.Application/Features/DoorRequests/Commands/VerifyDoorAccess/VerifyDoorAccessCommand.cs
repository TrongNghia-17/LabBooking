using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Commands.VerifyDoorAccess;

public record VerifyDoorAccessCommand(Guid RequestId) : IRequest<VerifyAccessResponse>;
