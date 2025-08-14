using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;

namespace BarberShopManagement.Business.Interfaces
{
    public interface IBarberService
    {
        Task<IEnumerable<Barber>> GetAllAsync();
        Task<Barber?> GetByIdAsync(int id);
        Task<Barber?> AuthenticateAsync(string username, string password);
        Task<int> CreateAsync(string username, string password, string fullName, string phone, int offDayOfWeek);
        Task<bool> UpdateAsync(int id, string username, string fullName, string phone, int offDayOfWeek);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Barber>> GetAvailableBarbersAsync(DateTime startTime, DateTime endTime);
        Task<bool> UpdateWithPasswordAsync(int id, string username, string fullName, string phone, int offDayOfWeek, string? newPassword = null);
    }
}
