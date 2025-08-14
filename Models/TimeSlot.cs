using System;

namespace BarberShopManagement.Models
{
    public class TimeSlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsMorning => StartTime.Hour < 13;
        public string DisplayText => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
        public string Session => IsMorning ? "Morning" : "Afternoon";
    }
}
