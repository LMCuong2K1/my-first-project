using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;

namespace BarberShopManagement.Business.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByUsernameAsync(string username);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> AuthenticateAsync(string username, string password);
        Task<int> RegisterAsync(string username, string password, string fullName, string phone, string email);
        Task<bool> UpdateAsync(int id, string username, string fullName, string phone, string email);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateWithPasswordAsync(int id, string username, string fullName, string phone, string email, string? newPassword = null);
    }
}
