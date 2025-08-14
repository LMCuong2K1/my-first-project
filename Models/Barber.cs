using System;

namespace BarberShopManagement.Models
{
    public class Barber
    {
        public int BarberId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int OffDayOfWeek { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
