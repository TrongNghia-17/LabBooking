using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetLabStatistics;

public record GetLabStatisticsQuery(int Year) : IRequest<IEnumerable<LabStatisticResponse>>;
