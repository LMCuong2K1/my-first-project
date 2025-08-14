using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using BarberShopManagement.Data;
using BarberShopManagement.Data.Interfaces;
using BarberShopManagement.Repositories;
using BarberShopManagement.Business.Interfaces;
using BarberShopManagement.Business.Services;
using BarberShopManagement.Presentation.Menus;
using System;
using System.IO;
using System.Text;

namespace BarberShopManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            // Cấu hình console
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
                Console.Title = "Barber Shop Management System";
            }
            catch
            {
                // Fallback nếu không set được encoding
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            DBHelper.Initialize(configuration);

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddScoped<IAdminRepository, AdminRepository>()
                .AddScoped<IBarberRepository, BarberRepository>()
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IServiceRepository, ServiceRepository>()
                .AddScoped<IBookingRepository, BookingRepository>()
                .AddScoped<IAdminService, AdminService>()
                .AddScoped<IBarberService, BarberService>()
                .AddScoped<ICustomerService, CustomerService>()
                .AddScoped<IServiceService, ServiceService>()
                .AddScoped<IBookingService, BookingService>()
                .AddTransient<MainMenu>()
                .AddTransient<AdminMenu>()
                .AddTransient<BarberMenu>()
                .AddTransient<CustomerMenu>()
                .BuildServiceProvider();

            try
            {
                var mainMenu = serviceProvider.GetRequiredService<MainMenu>();
                mainMenu.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi ứng dụng: {ex.Message}");
                Console.ReadKey();
            }
        }
    }
}
