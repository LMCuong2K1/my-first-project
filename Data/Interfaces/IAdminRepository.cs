using System.Threading.Tasks;
using BarberShopManagement.Models;

namespace BarberShopManagement.Data.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByUsernameAsync(string username);
        Task<Admin?> GetByIdAsync(int id);
        Task<int> AddAsync(Admin admin);
        Task<bool> UpdateAsync(Admin admin);
        Task<bool> DeleteAsync(int id);
    }
}
