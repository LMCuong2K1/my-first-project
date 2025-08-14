using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BarberShopManagement.Models;
using BarberShopManagement.Data.Interfaces;
using BarberShopManagement.Data;

namespace BarberShopManagement.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            var bookings = new List<Booking>();
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_booking_get_all", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                bookings.Add(new Booking
                {
                    BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                    CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? 0 : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    BarberId = reader.IsDBNull(reader.GetOrdinal("BarberId")) ? 0 : reader.GetInt32(reader.GetOrdinal("BarberId")),
                    ServiceId = reader.IsDBNull(reader.GetOrdinal("ServiceId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ServiceId")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                    EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }
            return bookings;
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_booking_get_by_id", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_booking_id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Booking
                {
                    BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                    CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? 0 : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    BarberId = reader.IsDBNull(reader.GetOrdinal("BarberId")) ? 0 : reader.GetInt32(reader.GetOrdinal("BarberId")),
                    ServiceId = reader.IsDBNull(reader.GetOrdinal("ServiceId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ServiceId")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                    EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };
            }
            return null;
        }

        public async Task<IEnumerable<Booking>> GetByCustomerIdAsync(int customerId)
        {
            var bookings = new List<Booking>();
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_booking_get_by_customer", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_customer_id", customerId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                bookings.Add(new Booking
                {
                    BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                    CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? 0 : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    BarberId = reader.IsDBNull(reader.GetOrdinal("BarberId")) ? 0 : reader.GetInt32(reader.GetOrdinal("BarberId")),
                    ServiceId = reader.IsDBNull(reader.GetOrdinal("ServiceId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ServiceId")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                    EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }
            return bookings;
        }

        public async Task<IEnumerable<Booking>> GetByBarberIdAsync(int barberId)
        {
            var bookings = new List<Booking>();
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_booking_get_by_barber", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_barber_id", barberId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                bookings.Add(new Booking
                {
                    BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                    CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? 0 : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    BarberId = reader.IsDBNull(reader.GetOrdinal("BarberId")) ? 0 : reader.GetInt32(reader.GetOrdinal("BarberId")),
                    ServiceId = reader.IsDBNull(reader.GetOrdinal("ServiceId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ServiceId")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                    EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }
            return bookings;
        }

        public async Task<IEnumerable<Booking>> GetByDateAsync(DateTime date)
        {
            var bookings = new List<Booking>();
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_booking_get_by_date", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_date", date.Date);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                bookings.Add(new Booking
                {
                    BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                    CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? 0 : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    BarberId = reader.IsDBNull(reader.GetOrdinal("BarberId")) ? 0 : reader.GetInt32(reader.GetOrdinal("BarberId")),
                    ServiceId = reader.IsDBNull(reader.GetOrdinal("ServiceId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ServiceId")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                    EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }
            return bookings;
        }

        public async Task<int> AddAsync(Booking booking)
        {
            using var connection = DBHelper.GetConnection();
            
            try
            {
                using var command = new MySqlCommand("sp_booking_create", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                
                command.Parameters.AddWithValue("@p_customer_id", booking.CustomerId);
                command.Parameters.AddWithValue("@p_barber_id", booking.BarberId);
                command.Parameters.AddWithValue("@p_service_id", booking.ServiceId);
                command.Parameters.AddWithValue("@p_start_time", booking.StartTime);
                command.Parameters.AddWithValue("@p_end_time", booking.EndTime);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return reader.GetInt32(reader.GetOrdinal("NewId"));
                }
                return 0;
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_booking_update_status", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_booking_id", id);
            command.Parameters.AddWithValue("@p_status", status);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = DBHelper.GetConnection();
            var query = "DELETE FROM Bookings WHERE BookingId = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}
