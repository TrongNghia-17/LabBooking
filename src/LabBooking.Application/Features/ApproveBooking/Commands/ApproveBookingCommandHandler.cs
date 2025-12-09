using LabBooking.Application.Features.ApproveBooking.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.ApproveBooking.Commands
{
    public class ApproveBookingCommandHandler(
    IBookingRepository bookingRepository
    ) : IRequestHandler<ApproveBookingCommand, bool>
    {
        public async Task<bool> Handle(ApproveBookingCommand request, CancellationToken token)
        {
            // 1. Lấy Booking
            var booking = await bookingRepository.GetBookingByIdWithSlotsAsync(request.BookingId);

            var isValidManager = await bookingRepository.CheckBookingIsBelongToThisManager(request.BookingId,request.ManagerId);
            if (booking == null || booking.Status != BookingStatus.Pending)
                throw new Exception("Đơn không tồn tại hoặc trạng thái không hợp lệ.");
            if (!isValidManager)
                throw new Exception("Bạn không có quyền duyệt đơn này.");
            // 2. Gọi Repo xử lý toàn bộ logic (Duyệt/Đè/Tạo Consent)
            await bookingRepository.ApproveBookingWithOverrideLogicAsync(booking);

            return true;
        }
    }
}

