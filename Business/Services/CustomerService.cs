using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;
using BarberShopManagement.Data.Interfaces;
using BarberShopManagement.Business.Interfaces;
using BarberShopManagement.Presentation.Utils;

namespace BarberShopManagement.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<bool> UpdateWithPasswordAsync(int id, string username, string fullName, string phone, string email, string? newPassword = null)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty");

            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                return false;
            }

            customer.Username = username;
            customer.FullName = fullName;
            customer.Phone = phone;
            customer.Email = email;

            string? newPasswordHash = null;
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                newPasswordHash = PasswordHasher.HashPassword(newPassword);
            }

            return await _customerRepository.UpdateWithPasswordAsync(customer, newPasswordHash);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<Customer?> GetByUsernameAsync(string username)
        {
            return await _customerRepository.GetByUsernameAsync(username);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _customerRepository.GetByEmailAsync(email);
        }

        public async Task<Customer?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            var customer = await _customerRepository.GetByUsernameAsync(username);

            if (customer == null || !PasswordHasher.VerifyPassword(password, customer.PasswordHash))
            {
                return null;
            }

            return customer;
        }

        public async Task<int> RegisterAsync(string username, string password, string fullName, string phone, string email)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty");

            var passwordHash = PasswordHasher.HashPassword(password);

            var customer = new Customer
            {
                Username = username,
                PasswordHash = passwordHash,
                FullName = fullName,
                Phone = phone,
                Email = email
            };

            return await _customerRepository.AddAsync(customer);
        }

        public async Task<bool> UpdateAsync(int id, string username, string fullName, string phone, string email)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty");

            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                return false;
            }

            customer.Username = username;
            customer.FullName = fullName;
            customer.Phone = phone;
            customer.Email = email;

            return await _customerRepository.UpdateAsync(customer);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _customerRepository.DeleteAsync(id);
        }
    }
}
