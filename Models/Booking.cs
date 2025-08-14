using System;

namespace BarberShopManagement.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public int BarberId { get; set; }
        public int ServiceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "Booked";
        public DateTime CreatedAt { get; set; }
    }
}
