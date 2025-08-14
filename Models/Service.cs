using System;

namespace BarberShopManagement.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
