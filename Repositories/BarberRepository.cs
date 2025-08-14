using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BarberShopManagement.Models;
using BarberShopManagement.Data.Interfaces;
using BarberShopManagement.Data;

namespace BarberShopManagement.Repositories
{
    public class BarberRepository : IBarberRepository
    {
        public async Task<IEnumerable<Barber>> GetAllAsync()
        {
            var barbers = new List<Barber>();
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_barber_get_all", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                barbers.Add(new Barber
                {
                    BarberId = reader.GetInt32(reader.GetOrdinal("BarberId")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    OffDayOfWeek = reader.GetInt32(reader.GetOrdinal("OffDayOfWeek")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }
            return barbers;
        }

        public async Task<Barber?> GetByIdAsync(int id)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_barber_get_by_id", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@p_barber_id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Barber
                {
                    BarberId = reader.GetInt32(reader.GetOrdinal("BarberId")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    OffDayOfWeek = reader.GetInt32(reader.GetOrdinal("OffDayOfWeek")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };
            }
            return null;
        }

        public async Task<Barber?> GetByUsernameAsync(string username)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_barber_get_by_username", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@p_username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Barber
                {
                    BarberId = reader.GetInt32(reader.GetOrdinal("BarberId")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    OffDayOfWeek = reader.GetInt32(reader.GetOrdinal("OffDayOfWeek")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };
            }
            return null;
        }

        public async Task<int> AddAsync(Barber barber)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_barber_create", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@p_username", barber.Username);
            command.Parameters.AddWithValue("@p_password_hash", barber.PasswordHash);
            command.Parameters.AddWithValue("@p_full_name", barber.FullName);
            command.Parameters.AddWithValue("@p_phone", barber.Phone);
            command.Parameters.AddWithValue("@p_off_day", barber.OffDayOfWeek);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32(reader.GetOrdinal("NewId"));
            }
            return 0;
        }

        public async Task<bool> UpdateAsync(Barber barber)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_barber_update", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@p_barber_id", barber.BarberId);
            command.Parameters.AddWithValue("@p_username", barber.Username);
            command.Parameters.AddWithValue("@p_full_name", barber.FullName);
            command.Parameters.AddWithValue("@p_phone", barber.Phone);
            command.Parameters.AddWithValue("@p_off_day", barber.OffDayOfWeek);

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
            using var command = new MySqlCommand("sp_barber_delete", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@p_barber_id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
            }
            return false;
        }

        public async Task<bool> IsAvailableAsync(int barberId, DateTime startTime, DateTime endTime)
        {
            using var connection = DBHelper.GetConnection();
            var query = "SELECT fn_check_barber_availability(@BarberId, @StartTime, @EndTime, 0) AS IsAvailable";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@BarberId", barberId);
            command.Parameters.AddWithValue("@StartTime", startTime);
            command.Parameters.AddWithValue("@EndTime", endTime);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToBoolean(result);
        }

        public async Task<IEnumerable<Barber>> GetAvailableBarbersAsync(DateTime startTime, DateTime endTime)
        {
            var barbers = new List<Barber>();
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_barber_get_available", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@p_start_time", startTime);
            command.Parameters.AddWithValue("@p_end_time", endTime);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                barbers.Add(new Barber
                {
                    BarberId = reader.GetInt32(reader.GetOrdinal("BarberId")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    OffDayOfWeek = reader.GetInt32(reader.GetOrdinal("OffDayOfWeek")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }
            return barbers;
        }

        // Method để update với mật khẩu
        public async Task<bool> UpdateWithPasswordAsync(Barber barber, string? newPasswordHash = null)
        {
            using var connection = DBHelper.GetConnection();

            try
            {
                using var command = new MySqlCommand("sp_barber_update_with_password", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@p_barber_id", barber.BarberId);
                command.Parameters.AddWithValue("@p_username", barber.Username);
                command.Parameters.AddWithValue("@p_full_name", barber.FullName);
                command.Parameters.AddWithValue("@p_phone", barber.Phone);
                command.Parameters.AddWithValue("@p_off_day", barber.OffDayOfWeek);
                command.Parameters.AddWithValue("@p_new_password_hash", newPasswordHash ?? (object)DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return Convert.ToInt32(reader["RowsAffected"]) > 0;
                }
                return false;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

    }
}
