using System;
using System.Collections.Generic;

namespace BarberShopManagement.Models
{
    public class BookingRequest
    {
        public int CustomerId { get; set; }
        public int BarberId { get; set; }
        public List<int> ServiceIds { get; set; } = new List<int>();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalDuration { get; set; }
        public string ServiceNames { get; set; } = string.Empty;
    }
}
