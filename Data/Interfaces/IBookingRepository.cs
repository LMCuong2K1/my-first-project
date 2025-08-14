using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;

namespace BarberShopManagement.Data.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task<IEnumerable<Booking>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Booking>> GetByBarberIdAsync(int barberId);
        Task<IEnumerable<Booking>> GetByDateAsync(DateTime date);
        Task<int> AddAsync(Booking booking);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> DeleteAsync(int id);
    }
}
