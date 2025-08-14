using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;

namespace BarberShopManagement.Data.Interfaces
{
    public interface IBarberRepository
    {
        Task<IEnumerable<Barber>> GetAllAsync();
        Task<Barber?> GetByIdAsync(int id);
        Task<Barber?> GetByUsernameAsync(string username);
        Task<int> AddAsync(Barber barber);
        Task<bool> UpdateAsync(Barber barber);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsAvailableAsync(int barberId, DateTime startTime, DateTime endTime);
        Task<IEnumerable<Barber>> GetAvailableBarbersAsync(DateTime startTime, DateTime endTime);
        Task<bool> UpdateWithPasswordAsync(Barber barber, string? newPasswordHash = null);
    }
}
