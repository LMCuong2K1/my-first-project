using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarberShopManagement.Models;
using BarberShopManagement.Data.Interfaces;
using BarberShopManagement.Business.Interfaces;

namespace BarberShopManagement.Business.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Booking>> GetByCustomerIdAsync(int customerId)
        {
            return await _bookingRepository.GetByCustomerIdAsync(customerId);
        }

        public async Task<IEnumerable<Booking>> GetByBarberIdAsync(int barberId)
        {
            return await _bookingRepository.GetByBarberIdAsync(barberId);
        }

        public async Task<IEnumerable<Booking>> GetByDateAsync(DateTime date)
        {
            return await _bookingRepository.GetByDateAsync(date);
        }

        public async Task<int> CreateAsync(int customerId, int barberId, int serviceId, DateTime startTime, DateTime endTime)
        {
            var booking = new Booking
            {
                CustomerId = customerId,
                BarberId = barberId,
                ServiceId = serviceId,
                StartTime = startTime,
                EndTime = endTime,
                Status = "Booked"
            };

            return await _bookingRepository.AddAsync(booking);
        }

        public async Task<bool> CancelAsync(int id)
        {
            return await _bookingRepository.UpdateStatusAsync(id, "Cancelled");
        }

        public async Task<bool> CompleteAsync(int id)
        {
            return await _bookingRepository.UpdateStatusAsync(id, "Completed");
        }

        public async Task<IEnumerable<TimeSlot>> GenerateTimeSlots(DateTime date, int durationMinutes)
        {
            var slots = new List<TimeSlot>();

            // Morning slots (8 AM - 12 PM)
            for (int hour = 8; hour < 12; hour++)
            {
                for (int minute = 0; minute < 60; minute += 30)
                {
                    var startTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                    var endTime = startTime.AddMinutes(durationMinutes);

                    if (endTime.Hour <= 12)
                    {
                        slots.Add(new TimeSlot
                        {
                            StartTime = startTime,
                            EndTime = endTime
                        });
                    }
                }
            }

            // Afternoon slots (1 PM - 5 PM)
            for (int hour = 13; hour < 17; hour++)
            {
                for (int minute = 0; minute < 60; minute += 30)
                {
                    var startTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                    var endTime = startTime.AddMinutes(durationMinutes);

                    if (endTime.Hour <= 17)
                    {
                        slots.Add(new TimeSlot
                        {
                            StartTime = startTime,
                            EndTime = endTime
                        });
                    }
                }
            }

            return slots;
        }
    }
}
