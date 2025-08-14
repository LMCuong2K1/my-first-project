using System.Threading.Tasks;
using BarberShopManagement.Models;

namespace BarberShopManagement.Business.Interfaces
{
    public interface IAdminService
    {
        Task<Admin?> AuthenticateAsync(string username, string password);
        Task<Admin?> GetByIdAsync(int id);
        Task<int> CreateAsync(string username, string password, string fullName);
        Task<bool> UpdateAsync(int id, string username, string fullName);
        Task<bool> DeleteAsync(int id);
        Task UpdateBookingByCustomerAsync(int customerId, string newStatus);
        Task UpdateBookingByBarberAsync(int barberId, string newStatus);
        Task UpdateBookingByServiceAsync(int serviceId, string newStatus);
        Task UpdateBookingByIdAsync(int bookingId, string newStatus);

    }
}
