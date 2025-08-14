using System;
using System.Threading.Tasks;
using BarberShopManagement.Models;
using BarberShopManagement.Data.Interfaces;
using BarberShopManagement.Business.Interfaces;
using BarberShopManagement.Presentation.Utils;

namespace BarberShopManagement.Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<Admin?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            try
            {
                var admin = await _adminRepository.GetByUsernameAsync(username);

                if (admin == null)
                {
                    return null;
                }

                bool passwordMatch = PasswordHasher.VerifyPassword(password, admin.PasswordHash);

                if (!passwordMatch)
                {
                    return null;
                }

                return admin;
            }
            catch (Exception ex)
            {
                // Có thể log lỗi vào file thay vì console nếu cần
                return null;
            }
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            return await _adminRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateAsync(string username, string password, string fullName)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty");

            var passwordHash = PasswordHasher.HashPassword(password);

            var admin = new Admin
            {
                Username = username,
                PasswordHash = passwordHash,
                FullName = fullName
            };

            return await _adminRepository.AddAsync(admin);
        }

        public async Task<bool> UpdateAsync(int id, string username, string fullName)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty");

            var admin = await _adminRepository.GetByIdAsync(id);

            if (admin == null)
            {
                return false;
            }

            admin.Username = username;
            admin.FullName = fullName;

            return await _adminRepository.UpdateAsync(admin);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _adminRepository.DeleteAsync(id);
        }
    }
}
