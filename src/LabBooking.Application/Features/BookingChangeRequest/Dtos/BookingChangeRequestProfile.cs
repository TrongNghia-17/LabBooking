using LabBooking.Application.Features.Booking.Dtos;
using LabBooking.Application.Features.BookingChangeRequest.Commands.CreateBookingChangeRequest;
using LabBooking.Application.Features.BookingPriorityDetail.Dtos;
using LabBooking.Application.Features.BookingSlots.Dtos;
using LabBooking.Application.Features.ExternalEquipment.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Dtos
{
    public class BookingChangeRequestProfile : Profile
    {
        public BookingChangeRequestProfile()
        {
            CreateMap<Domain.Entities.BookingChangeRequest, BookingChangeRequestResponse>()
                // Map các trường cơ bản tự động (vì trùng tên: NewTitle -> NewTitle)
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.ManagerReason.ToString()))

                // --- MAP CÁC TRƯỜNG JSON ---

                // 1. Project: String JSON -> Object
                .ForMember(dest => dest.NewProject, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.NewProjectJson)
                    ? null
                    : JsonSerializer.Deserialize<ProjectResponse>(src.NewProjectJson, (JsonSerializerOptions)null)))

                // 2. Priority: String JSON -> Object
                .ForMember(dest => dest.NewPriorityDetail, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.NewPriorityDetailJson)
                    ? null
                    : JsonSerializer.Deserialize<BookingPriorityDetailResponse>(src.NewPriorityDetailJson, (JsonSerializerOptions)null)))

                // 3. Equipment: String JSON -> List Object
                .ForMember(dest => dest.NewExternalEquipments, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.NewExternalEquipmentsJson)
                    ? new List<ExternalEquipmentResponse>()
                    : JsonSerializer.Deserialize<List<ExternalEquipmentResponse>>(src.NewExternalEquipmentsJson, (JsonSerializerOptions)null)))

                .ForMember(dest => dest.NewOutSideGuests, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.NewOutSideGuestsJson)
                ? new List<OutSideGuestResponse>()
                : JsonSerializer.Deserialize<List<OutSideGuestResponse>>(src.NewOutSideGuestsJson, (JsonSerializerOptions?)null)))
                
                // --- MAP THÔNG TIN TỪ BOOKING GỐC ---
                // (Để FE biết đơn này thuộc phòng nào, loại gì)
                .ForMember(dest => dest.OriginalType, opt => opt.MapFrom(src => src.Booking.Type.ToString()))
                .ForMember(dest => dest.LabRoomId, opt => opt.MapFrom(src => src.Booking.LabRoomId))
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Booking.LabRoom.LabName)); // Cần Include LabRoom trong Repo
        }
    }
}
