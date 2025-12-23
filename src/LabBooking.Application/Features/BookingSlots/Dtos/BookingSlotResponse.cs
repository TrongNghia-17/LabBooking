using LabBooking.Domain.Entities;
using Medo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingSlots.Dtos
{
    public class BookingSlotResponse
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public DateOnly Date { get; set; }
        public Guid SlotId { get; set; }
        public string Reason { get; set; } // "Booked" hoặc "Maintenance"
        public int Priority { get; set; }
        public string Status { get; set; }

        // --- THÊM DÒNG NÀY ---
        public bool IsMyBooking { get; set; } = false;
        public string? Title { get; set; }
        public string? BookerName { get; set; }
        public string? Description { get; set; }
        public string? TypeLabel { get; set; }
    }
}
