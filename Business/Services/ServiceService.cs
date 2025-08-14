using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;
using BarberShopManagement.Data.Interfaces;
using BarberShopManagement.Business.Interfaces;

namespace BarberShopManagement.Business.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _serviceRepository.GetAllAsync();
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _serviceRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateAsync(string name, string description, decimal price, int durationMinutes)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Service name cannot be empty");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Service description cannot be empty");

            if (price <= 0)
                throw new ArgumentException("Price must be greater than 0");

            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            var service = new Service
            {
                Name = name,
                Description = description,
                Price = price,
                DurationMinutes = durationMinutes
            };

            return await _serviceRepository.AddAsync(service);
        }

        public async Task<bool> UpdateAsync(int id, string name, string description, decimal price, int durationMinutes)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Service name cannot be empty");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Service description cannot be empty");

            if (price <= 0)
                throw new ArgumentException("Price must be greater than 0");

            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            var service = await _serviceRepository.GetByIdAsync(id);

            if (service == null)
            {
                return false;
            }

            service.Name = name;
            service.Description = description;
            service.Price = price;
            service.DurationMinutes = durationMinutes;

            return await _serviceRepository.UpdateAsync(service);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _serviceRepository.DeleteAsync(id);
        }
    }
}
