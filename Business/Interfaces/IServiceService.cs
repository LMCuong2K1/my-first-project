using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;

namespace BarberShopManagement.Business.Interfaces
{
    public interface IServiceService
    {
        Task<IEnumerable<Service>> GetAllAsync();
        Task<Service?> GetByIdAsync(int id);
        Task<int> CreateAsync(string name, string description, decimal price, int durationMinutes);
        Task<bool> UpdateAsync(int id, string name, string description, decimal price, int durationMinutes);
        Task<bool> DeleteAsync(int id);
    }
}
