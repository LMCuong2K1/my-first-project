using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;

namespace BarberShopManagement.Business.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task<IEnumerable<Booking>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Booking>> GetByBarberIdAsync(int barberId);
        Task<IEnumerable<Booking>> GetByDateAsync(DateTime date);
        Task<int> CreateAsync(int customerId, int barberId, int serviceId, DateTime startTime, DateTime endTime);
        Task<bool> CancelAsync(int id);
        Task<bool> CompleteAsync(int id);
        Task<IEnumerable<TimeSlot>> GenerateTimeSlots(DateTime date, int durationMinutes);
    }
}
