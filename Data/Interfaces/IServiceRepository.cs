using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;

namespace BarberShopManagement.Data.Interfaces
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetAllAsync();
        Task<Service?> GetByIdAsync(int id);
        Task<int> AddAsync(Service service);
        Task<bool> UpdateAsync(Service service);
        Task<bool> DeleteAsync(int id);
    }
}
