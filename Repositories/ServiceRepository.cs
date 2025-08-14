using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BarberShopManagement.Models;
using BarberShopManagement.Data.Interfaces;
using BarberShopManagement.Data;

namespace BarberShopManagement.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            var services = new List<Service>();
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_service_get_all", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                services.Add(new Service
                {
                    ServiceId = reader.GetInt32(reader.GetOrdinal("ServiceId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    DurationMinutes = reader.GetInt32(reader.GetOrdinal("DurationMinutes")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }
            return services;
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_service_get_by_id", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_service_id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Service
                {
                    ServiceId = reader.GetInt32(reader.GetOrdinal("ServiceId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    DurationMinutes = reader.GetInt32(reader.GetOrdinal("DurationMinutes")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };
            }
            return null;
        }

        public async Task<int> AddAsync(Service service)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_service_create", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_name", service.Name);
            command.Parameters.AddWithValue("@p_description", service.Description);
            command.Parameters.AddWithValue("@p_price", service.Price);
            command.Parameters.AddWithValue("@p_duration_minutes", service.DurationMinutes);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32(reader.GetOrdinal("NewId"));
            }
            return 0;
        }

        public async Task<bool> UpdateAsync(Service service)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_service_update", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_service_id", service.ServiceId);
            command.Parameters.AddWithValue("@p_name", service.Name);
            command.Parameters.AddWithValue("@p_description", service.Description);
            command.Parameters.AddWithValue("@p_price", service.Price);
            command.Parameters.AddWithValue("@p_duration_minutes", service.DurationMinutes);

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
            using var command = new MySqlCommand("sp_service_delete", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_service_id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
            }
            return false;
        }
    }
}
