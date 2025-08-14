using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;

namespace BarberShopManagement.Data.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByUsernameAsync(string username);
        Task<Customer?> GetByEmailAsync(string email);
        Task<int> AddAsync(Customer customer);
        Task<bool> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateWithPasswordAsync(Customer customer, string? newPasswordHash = null);
    }
}
