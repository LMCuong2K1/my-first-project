using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BarberShopManagement.Models;
using BarberShopManagement.Data.Interfaces;
using BarberShopManagement.Data;

namespace BarberShopManagement.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var customers = new List<Customer>();
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_customer_get_all", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                customers.Add(new Customer
                {
                    CustomerId = Convert.ToInt32(reader["CustomerId"]),
                    Username = reader["Username"]?.ToString() ?? string.Empty,
                    FullName = reader["FullName"]?.ToString() ?? string.Empty,
                    Phone = reader["Phone"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }
            return customers;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_customer_get_by_id", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_customer_id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Customer
                {
                    CustomerId = Convert.ToInt32(reader["CustomerId"]),
                    Username = reader["Username"]?.ToString() ?? string.Empty,
                    PasswordHash = reader["PasswordHash"]?.ToString() ?? string.Empty,
                    FullName = reader["FullName"]?.ToString() ?? string.Empty,
                    Phone = reader["Phone"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                };
            }
            return null;
        }

        public async Task<Customer?> GetByUsernameAsync(string username)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_customer_get_by_username", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Customer
                {
                    CustomerId = Convert.ToInt32(reader["CustomerId"]),
                    Username = reader["Username"]?.ToString() ?? string.Empty,
                    PasswordHash = reader["PasswordHash"]?.ToString() ?? string.Empty,
                    FullName = reader["FullName"]?.ToString() ?? string.Empty,
                    Phone = reader["Phone"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                };
            }
            return null;
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_customer_get_by_email", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Customer
                {
                    CustomerId = Convert.ToInt32(reader["CustomerId"]),
                    Username = reader["Username"]?.ToString() ?? string.Empty,
                    PasswordHash = reader["PasswordHash"]?.ToString() ?? string.Empty,
                    FullName = reader["FullName"]?.ToString() ?? string.Empty,
                    Phone = reader["Phone"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                };
            }
            return null;
        }

        public async Task<int> AddAsync(Customer customer)
        {
            using var connection = DBHelper.GetConnection();
            
            try
            {
                using var command = new MySqlCommand("sp_customer_create", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                
                command.Parameters.AddWithValue("@p_username", customer.Username);
                command.Parameters.AddWithValue("@p_password_hash", customer.PasswordHash);
                command.Parameters.AddWithValue("@p_full_name", customer.FullName);
                command.Parameters.AddWithValue("@p_phone", customer.Phone);
                command.Parameters.AddWithValue("@p_email", customer.Email);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var newId = Convert.ToInt32(reader["NewId"]);
                    var errorMessage = reader.IsDBNull(reader.GetOrdinal("ErrorMessage")) ? 
                        null : reader["ErrorMessage"]?.ToString();
                    
                    if (errorMessage != null && errorMessage.Contains("Duplicate"))
                        return -1; // Duplicate error
                    
                    return newId;
                }
                return 0;
            }
            catch (MySqlException ex)
            {
                // ✅ SỬA: Trả về mã lỗi chi tiết
                if (ex.Message.Contains("Username already exists"))
                    return -1; // Username trùng
                if (ex.Message.Contains("Phone number already exists"))
                    return -2; // Phone trùng
                if (ex.Message.Contains("Email already exists"))
                    return -3; // Email trùng
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            using var connection = DBHelper.GetConnection();
            
            try
            {
                using var command = new MySqlCommand("sp_customer_update", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                
                command.Parameters.AddWithValue("@p_customer_id", customer.CustomerId);
                command.Parameters.AddWithValue("@p_username", customer.Username);
                command.Parameters.AddWithValue("@p_full_name", customer.FullName);
                command.Parameters.AddWithValue("@p_phone", customer.Phone);
                command.Parameters.AddWithValue("@p_email", customer.Email);

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

        // ✅ MỚI: Update với mật khẩu
        public async Task<bool> UpdateWithPasswordAsync(Customer customer, string? newPasswordHash = null)
        {
            using var connection = DBHelper.GetConnection();
            
            try
            {
                using var command = new MySqlCommand("sp_customer_update_with_password", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                
                command.Parameters.AddWithValue("@p_customer_id", customer.CustomerId);
                command.Parameters.AddWithValue("@p_username", customer.Username);
                command.Parameters.AddWithValue("@p_full_name", customer.FullName);
                command.Parameters.AddWithValue("@p_phone", customer.Phone);
                command.Parameters.AddWithValue("@p_email", customer.Email);
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

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = DBHelper.GetConnection();
            using var command = new MySqlCommand("sp_customer_delete", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@p_customer_id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Convert.ToInt32(reader["RowsAffected"]) > 0;
            }
            return false;
        }
    }
}
