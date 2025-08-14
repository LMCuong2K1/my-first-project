using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BarberShopManagement.Models;
using BarberShopManagement.Data.Interfaces;
using BarberShopManagement.Data;

namespace BarberShopManagement.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            using var connection = DBHelper.GetConnection();
            
            var query = "SELECT AdminId, Username, PasswordHash, FullName, CreatedAt, UpdatedAt FROM Admins WHERE Username = @Username AND IsDeleted = FALSE";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Admin
                {
                    AdminId = Convert.ToInt32(reader["AdminId"]),
                    Username = reader["Username"]?.ToString() ?? string.Empty,
                    PasswordHash = reader["PasswordHash"]?.ToString() ?? string.Empty,
                    FullName = reader["FullName"]?.ToString() ?? string.Empty,
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                };
            }
            return null;
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            using var connection = DBHelper.GetConnection();
            var query = "SELECT AdminId, Username, PasswordHash, FullName, CreatedAt, UpdatedAt FROM Admins WHERE AdminId = @Id AND IsDeleted = FALSE";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Admin
                {
                    AdminId = Convert.ToInt32(reader["AdminId"]),
                    Username = reader["Username"]?.ToString() ?? string.Empty,
                    PasswordHash = reader["PasswordHash"]?.ToString() ?? string.Empty,
                    FullName = reader["FullName"]?.ToString() ?? string.Empty,
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                };
            }
            return null;
        }

        public async Task<int> AddAsync(Admin admin)
        {
            using var connection = DBHelper.GetConnection();
            var query = "INSERT INTO Admins (Username, PasswordHash, FullName) VALUES (@Username, @PasswordHash, @FullName); SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", admin.Username);
            command.Parameters.AddWithValue("@PasswordHash", admin.PasswordHash);
            command.Parameters.AddWithValue("@FullName", admin.FullName);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(Admin admin)
        {
            using var connection = DBHelper.GetConnection();
            var query = "UPDATE Admins SET Username = @Username, FullName = @FullName WHERE AdminId = @AdminId AND IsDeleted = FALSE";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", admin.Username);
            command.Parameters.AddWithValue("@FullName", admin.FullName);
            command.Parameters.AddWithValue("@AdminId", admin.AdminId);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = DBHelper.GetConnection();
            var query = "UPDATE Admins SET IsDeleted = TRUE WHERE AdminId = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}
