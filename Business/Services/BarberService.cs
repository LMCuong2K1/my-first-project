using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;
using BarberShopManagement.Data.Interfaces;
using BarberShopManagement.Business.Interfaces;
using BarberShopManagement.Presentation.Utils;

namespace BarberShopManagement.Business.Services
{
    public class BarberService : IBarberService
    {
        private readonly IBarberRepository _barberRepository;

        public BarberService(IBarberRepository barberRepository)
        {
            _barberRepository = barberRepository;
        }

        public async Task<IEnumerable<Barber>> GetAllAsync()
        {
            return await _barberRepository.GetAllAsync();
        }

        public async Task<Barber?> GetByIdAsync(int id)
        {
            return await _barberRepository.GetByIdAsync(id);
        }

        public async Task<Barber?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            var barber = await _barberRepository.GetByUsernameAsync(username);

            if (barber == null || !PasswordHasher.VerifyPassword(password, barber.PasswordHash))
            {
                return null;
            }

            return barber;
        }

        public async Task<int> CreateAsync(string username, string password, string fullName, string phone, int offDayOfWeek)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty");

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be empty");

            if (offDayOfWeek < 1 || offDayOfWeek > 7)
                throw new ArgumentException("Off day of week must be between 1 and 7");

            var passwordHash = PasswordHasher.HashPassword(password);

            var barber = new Barber
            {
                Username = username,
                PasswordHash = passwordHash,
                FullName = fullName,
                Phone = phone,
                OffDayOfWeek = offDayOfWeek
            };

            return await _barberRepository.AddAsync(barber);
        }

        public async Task<bool> UpdateAsync(int id, string username, string fullName, string phone, int offDayOfWeek)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty");

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be empty");

            if (offDayOfWeek < 1 || offDayOfWeek > 7)
                throw new ArgumentException("Off day of week must be between 1 and 7");

            var barber = await _barberRepository.GetByIdAsync(id);

            if (barber == null)
            {
                return false;
            }

            barber.Username = username;
            barber.FullName = fullName;
            barber.Phone = phone;
            barber.OffDayOfWeek = offDayOfWeek;

            return await _barberRepository.UpdateAsync(barber);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _barberRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Barber>> GetAvailableBarbersAsync(DateTime startTime, DateTime endTime)
        {
            return await _barberRepository.GetAvailableBarbersAsync(startTime, endTime);
        }
        public async Task<bool> UpdateWithPasswordAsync(int id, string username, string fullName, string phone, int offDayOfWeek, string? newPassword = null)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty");

            var barber = await _barberRepository.GetByIdAsync(id);

            if (barber == null)
            {
                return false;
            }

            barber.Username = username;
            barber.FullName = fullName;
            barber.Phone = phone;
            barber.OffDayOfWeek = offDayOfWeek;

            string? newPasswordHash = null;
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                newPasswordHash = PasswordHasher.HashPassword(newPassword);
            }

            return await _barberRepository.UpdateWithPasswordAsync(barber, newPasswordHash);
        }
    }
}
