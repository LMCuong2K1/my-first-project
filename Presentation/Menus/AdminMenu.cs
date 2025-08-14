using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using BarberShopManagement.Business.Interfaces;
using BarberShopManagement.Models;
using BarberShopManagement.Presentation.Utils;
using MySql.Data.MySqlClient;
using BarberShopManagement.Data;

namespace BarberShopManagement.Presentation.Menus
{
    public class AdminMenu
    {
        private readonly IAdminService _adminService;
        private readonly IBarberService _barberService;
        private readonly ICustomerService _customerService;
        private readonly IServiceService _serviceService;
        private readonly IBookingService _bookingService;

        private Admin? _currentAdmin;

        public AdminMenu(
            IAdminService adminService,
            IBarberService barberService,
            ICustomerService customerService,
            IServiceService serviceService,
            IBookingService bookingService)
        {
            _adminService = adminService;
            _barberService = barberService;
            _customerService = customerService;
            _serviceService = serviceService;
            _bookingService = bookingService;
        }

        public async Task ShowLoginAsync()
        {
            UIHelper.DrawHeader("ĐĂNG NHẬP ADMIN");

            try
            {
                string username = UIHelper.GetInput("Tên đăng nhập");
                if (string.IsNullOrWhiteSpace(username))
                {
                    UIHelper.ShowError("Tên đăng nhập không được để trống!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                string password = ConsoleHelper.GetValidPassword("Mật khẩu: ");

                _currentAdmin = await _adminService.AuthenticateAsync(username, password);

                if (_currentAdmin != null)
                {
                    UIHelper.ShowSuccess($"Đăng nhập thành công! Xin chào, {_currentAdmin.FullName}");
                    UIHelper.WaitForAnyKey();
                    await ShowMenuAsync();
                }
                else
                {
                    UIHelper.ShowError("Đăng nhập thất bại! Tên đăng nhập hoặc mật khẩu không chính xác.");
                    UIHelper.WaitForAnyKey();
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi đăng nhập: {ex.Message}");
                UIHelper.WaitForAnyKey();
            }
        }

        private async Task ShowMenuAsync()
        {
            while (true)
            {
                UIHelper.DrawHeader($"PANEL ADMIN - {_currentAdmin!.FullName}");

                string[] menuItems = {
                    "Quản lý Barber",
                    "Quản lý Khách hàng",
                    "Quản lý Dịch vụ",
                    "Quản lý booking",
                    "Xem báo cáo"
                };

                UIHelper.DrawMenu(menuItems, "MENU ADMIN");

                string choice = UIHelper.GetInput("Lựa chọn của bạn");
                switch (choice)
                {
                    case "1":
                        await ManageBarberAsync();
                        break;
                    case "2":
                        await ManageCustomerAsync();
                        break;
                    case "3":
                        await ManageServiceAsync();
                        break;
                    case "4":
                        await ManageBookings();
                        break;
                    case "5":
                        await ViewReportsAsync();
                        break;
                    case "0":
                        _currentAdmin = null;
                        return;
                    default:
                        UIHelper.ShowError("Lựa chọn không hợp lệ!");
                        UIHelper.WaitForAnyKey();
                        break;
                }
            }
        }

        // =================== QUẢN LÝ BARBER ===================
        private async Task ManageBarberAsync()
        {
            while (true)
            {
                UIHelper.DrawHeader("QUẢN LÝ BARBER");

                string[] menuItems = {
                    "Xem danh sách Barber",
                    "Thêm Barber mới",
                    "Cập nhật Barber",
                    "Xóa Barber"
                };

                UIHelper.DrawMenu(menuItems, "QUẢN LÝ BARBER");

                string choice = UIHelper.GetInput("Lựa chọn của bạn");
                switch (choice)
                {
                    case "1":
                        await ViewBarbersAsync();
                        break;
                    case "2":
                        await AddBarberAsync();
                        break;
                    case "3":
                        await UpdateBarberAsync();
                        break;
                    case "4":
                        await DeleteBarberAsync();
                        break;
                    case "0":
                        return;
                    default:
                        UIHelper.ShowError("Lựa chọn không hợp lệ!");
                        UIHelper.WaitForAnyKey();
                        break;
                }
            }
        }

        private async Task ViewBarbersAsync()
        {
            try
            {
                UIHelper.DrawHeader("DANH SÁCH BARBER");
                var barbers = await _barberService.GetAllAsync();

                if (!barbers.Any())
                {
                    UIHelper.ShowWarning("Không có Barber nào trong hệ thống!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var headers = new[] { "ID", "Username", "Họ tên", "Số điện thoại", "Ngày nghỉ" };
                var rows = barbers.Select(b => new[]
                {
                    b.BarberId.ToString(),
                    b.Username,
                    b.FullName,
                    b.Phone,
                    GetDayName(b.OffDayOfWeek)
                }).ToArray();

                UIHelper.DrawTable(headers, rows);
                UIHelper.ShowInfo($"Tổng số Barber: {barbers.Count()}");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải danh sách Barber: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task AddBarberAsync()
        {
            UIHelper.DrawHeader("THÊM BARBER MỚI");

            try
            {
                string username = UIHelper.GetInput("Tên đăng nhập");
                if (string.IsNullOrWhiteSpace(username))
                {
                    UIHelper.ShowError("Tên đăng nhập không được để trống!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                string password = ConsoleHelper.GetValidPassword("Mật khẩu: ");
                string fullName = UIHelper.GetInput("Họ tên");
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    UIHelper.ShowError("Họ tên không được để trống!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                string phone = ValidationHelper.GetValidPhone("Số điện thoại: ");

                Console.WriteLine("\nNgày nghỉ trong tuần:");
                for (int i = 1; i <= 7; i++)
                {
                    Console.WriteLine($"  {i}. {GetDayName(i)}");
                }

                int offDayOfWeek = ValidationHelper.GetValidInt("Chọn ngày nghỉ: ", 1, 7);

                int barberId = await _barberService.CreateAsync(username, password, fullName, phone, offDayOfWeek);

                if (barberId > 0)
                {
                    UIHelper.ShowSuccess($"Thêm Barber thành công với ID: {barberId}");
                }
                else
                {
                    UIHelper.ShowError("Thêm Barber thất bại!");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi thêm Barber: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task UpdateBarberAsync()
{
    UIHelper.DrawHeader("CẬP NHẬT THÔNG TIN BARBER");

    try
    {
        string idInput = UIHelper.GetInput("Nhập ID Barber cần cập nhật");
        if (!int.TryParse(idInput, out int barberId))
        {
            UIHelper.ShowError("ID không hợp lệ!");
            UIHelper.WaitForAnyKey();
            return;
        }

        var barber = await _barberService.GetByIdAsync(barberId);

        if (barber == null)
        {
            UIHelper.ShowError($"Không tìm thấy Barber với ID: {barberId}");
            UIHelper.WaitForAnyKey();
            return;
        }

        Console.WriteLine($"\nThông tin hiện tại:");
        Console.WriteLine($"Tên đăng nhập: {barber.Username}");
        Console.WriteLine($"Họ tên: {barber.FullName}");
        Console.WriteLine($"Số điện thoại: {barber.Phone}");
        Console.WriteLine($"Ngày nghỉ: {GetDayName(barber.OffDayOfWeek)}");

        Console.WriteLine("\n--- Nhập thông tin mới (Enter để giữ nguyên) ---");

        string username = UIHelper.GetInput("Tên đăng nhập mới");
        username = string.IsNullOrEmpty(username) ? barber.Username : username;

        string fullName = UIHelper.GetInput("Họ tên mới");
        fullName = string.IsNullOrEmpty(fullName) ? barber.FullName : fullName;

        Console.Write("Số điện thoại mới: ");
        string phoneInput = Console.ReadLine()?.Trim();
        string phone = barber.Phone;
        if (!string.IsNullOrEmpty(phoneInput))
        {
            if (ValidationHelper.IsValidPhone(phoneInput))
            {
                phone = phoneInput;
            }
            else
            {
                UIHelper.ShowError("Số điện thoại không hợp lệ! Giữ nguyên số cũ.");
            }
        }

        Console.WriteLine($"Ngày nghỉ hiện tại: {GetDayName(barber.OffDayOfWeek)}");
        Console.WriteLine("1=Chủ Nhật, 2=Thứ Hai, ..., 7=Thứ Bảy");
        string offDayInput = UIHelper.GetInput("Chọn ngày nghỉ mới (Enter để giữ nguyên)");
        int offDayOfWeek = barber.OffDayOfWeek;
        if (!string.IsNullOrEmpty(offDayInput) && int.TryParse(offDayInput, out int newOffDay) && newOffDay >= 1 && newOffDay <= 7)
        {
            offDayOfWeek = newOffDay;
        }

        // ✅ THÊM PHẦN ĐỔI MẬT KHẨU
        string newPassword = null;
        string changePassword = UIHelper.GetInput("Có muốn đổi mật khẩu? (Y/N)");
        if (changePassword.ToUpper() == "Y")
        {
            newPassword = ConsoleHelper.GetValidPassword("Mật khẩu mới: ");
        }

        bool success = await _barberService.UpdateWithPasswordAsync(
            barberId, 
            username, 
            fullName, 
            phone, 
            offDayOfWeek, 
            newPassword
        );

        if (success)
        {
            UIHelper.ShowSuccess("Cập nhật thông tin Barber thành công!");
        }
        else
        {
            UIHelper.ShowError("Cập nhật thông tin Barber thất bại!");
        }
    }
    catch (Exception ex)
    {
        UIHelper.ShowError($"Lỗi khi cập nhật Barber: {ex.Message}");
    }

    UIHelper.WaitForAnyKey();
}


        private async Task DeleteBarberAsync()
        {
            UIHelper.DrawHeader("XÓA BARBER");

            try
            {
                string idInput = UIHelper.GetInput("Nhập ID Barber cần xóa");
                if (!int.TryParse(idInput, out int barberId))
                {
                    UIHelper.ShowError("ID không hợp lệ!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var barber = await _barberService.GetByIdAsync(barberId);

                if (barber == null)
                {
                    UIHelper.ShowError($"Không tìm thấy Barber với ID: {barberId}");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                Console.WriteLine($"\nThông tin Barber sẽ bị xóa:");
                Console.WriteLine($"ID: {barber.BarberId}");
                Console.WriteLine($"Tên đăng nhập: {barber.Username}");
                Console.WriteLine($"Họ tên: {barber.FullName}");
                Console.WriteLine($"Số điện thoại: {barber.Phone}");

                UIHelper.ShowWarning("CẢNH BÁO: Việc xóa Barber sẽ đánh dấu xóa (soft delete)!");
                
                string confirm = UIHelper.GetInput($"Bạn có chắc chắn muốn xóa Barber: {barber.FullName} (ID: {barber.BarberId})? (Y/N)");
                
                if (confirm.ToUpper() == "Y")
                {
                    bool success = await _barberService.DeleteAsync(barberId);

                    if (success)
                    {
                        UIHelper.ShowSuccess("Xóa Barber thành công!");
                    }
                    else
                    {
                        UIHelper.ShowError("Xóa Barber thất bại!");
                    }
                }
                else
                {
                    UIHelper.ShowInfo("Hủy xóa Barber.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi xóa Barber: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        // =================== QUẢN LÝ KHÁCH HÀNG ===================
        private async Task ManageCustomerAsync()
        {
            while (true)
            {
                UIHelper.DrawHeader("QUẢN LÝ KHÁCH HÀNG");

                string[] menuItems = {
                    "Xem danh sách Khách hàng",
                    "Thêm Khách hàng mới",
                    "Cập nhật Khách hàng",
                    "Xóa Khách hàng"
                };

                UIHelper.DrawMenu(menuItems, "QUẢN LÝ KHÁCH HÀNG");

                string choice = UIHelper.GetInput("Lựa chọn của bạn");
                switch (choice)
                {
                    case "1":
                        await ViewCustomersAsync();
                        break;
                    case "2":
                        await AddCustomerAsync();
                        break;
                    case "3":
                        await UpdateCustomerAsync();
                        break;
                    case "4":
                        await DeleteCustomerAsync();
                        break;
                    case "0":
                        return;
                    default:
                        UIHelper.ShowError("Lựa chọn không hợp lệ!");
                        UIHelper.WaitForAnyKey();
                        break;
                }
            }
        }

        private async Task ViewCustomersAsync()
        {
            try
            {
                UIHelper.DrawHeader("DANH SÁCH KHÁCH HÀNG");
                var customers = await _customerService.GetAllAsync();

                if (!customers.Any())
                {
                    UIHelper.ShowWarning("Không có Khách hàng nào trong hệ thống!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var headers = new[] { "ID", "Username", "Họ tên", "Số điện thoại", "Email" };
                var rows = customers.Select(c => new[]
                {
                    c.CustomerId.ToString(),
                    c.Username,
                    c.FullName,
                    c.Phone,
                    c.Email
                }).ToArray();

                UIHelper.DrawTable(headers, rows);
                UIHelper.ShowInfo($"Tổng số Khách hàng: {customers.Count()}");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải danh sách Khách hàng: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task AddCustomerAsync()
        {
            UIHelper.DrawHeader("THÊM KHÁCH HÀNG MỚI");

            try
            {
                string username = UIHelper.GetInput("Tên đăng nhập");
                if (string.IsNullOrWhiteSpace(username))
                {
                    UIHelper.ShowError("Tên đăng nhập không được để trống!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                string password = ConsoleHelper.GetValidPassword("Mật khẩu: ");
                string fullName = UIHelper.GetInput("Họ tên");
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    UIHelper.ShowError("Họ tên không được để trống!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                string phone = ValidationHelper.GetValidPhone("Số điện thoại: ");
                string email = ValidationHelper.GetValidEmail("Email: ");

                int result = await _customerService.RegisterAsync(username, password, fullName, phone, email);

                if (result > 0)
                {
                    UIHelper.ShowSuccess($"Thêm Khách hàng thành công với ID: {result}");
                }
                else if (result == -1)
                {
                    UIHelper.ShowError("Tên đăng nhập đã tồn tại!");
                }
                else if (result == -2)
                {
                    UIHelper.ShowError("Số điện thoại đã tồn tại!");
                }
                else if (result == -3)
                {
                    UIHelper.ShowError("Email đã tồn tại!");
                }
                else
                {
                    UIHelper.ShowError("Thêm Khách hàng thất bại!");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi thêm Khách hàng: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }


private async Task UpdateCustomerAsync()
{
    UIHelper.DrawHeader("CẬP NHẬT THÔNG TIN KHÁCH HÀNG");

    try
    {
        string idInput = UIHelper.GetInput("Nhập ID Khách hàng cần cập nhật");
        if (!int.TryParse(idInput, out int customerId))
        {
            UIHelper.ShowError("ID không hợp lệ!");
            UIHelper.WaitForAnyKey();
            return;
        }

        var customer = await _customerService.GetByIdAsync(customerId);

        if (customer == null)
        {
            UIHelper.ShowError($"Không tìm thấy Khách hàng với ID: {customerId}");
            UIHelper.WaitForAnyKey();
            return;
        }

        Console.WriteLine($"\nThông tin hiện tại:");
        Console.WriteLine($"Tên đăng nhập: {customer.Username}");
        Console.WriteLine($"Họ tên: {customer.FullName}");
        Console.WriteLine($"Số điện thoại: {customer.Phone}");
        Console.WriteLine($"Email: {customer.Email}");

        Console.WriteLine("\n--- Nhập thông tin mới (Enter để giữ nguyên) ---");

        string username = UIHelper.GetInput("Tên đăng nhập mới");
        username = string.IsNullOrEmpty(username) ? customer.Username : username;

        string fullName = UIHelper.GetInput("Họ tên mới");
        fullName = string.IsNullOrEmpty(fullName) ? customer.FullName : fullName;

        Console.Write("Số điện thoại mới: ");
        string phoneInput = Console.ReadLine()?.Trim();
        string phone = customer.Phone;
        if (!string.IsNullOrEmpty(phoneInput))
        {
            if (ValidationHelper.IsValidPhone(phoneInput))
            {
                phone = phoneInput;
            }
            else
            {
                UIHelper.ShowError("Số điện thoại không hợp lệ! Giữ nguyên số cũ.");
            }
        }

        Console.Write("Email mới: ");
        string emailInput = Console.ReadLine()?.Trim();
        string email = customer.Email;
        if (!string.IsNullOrEmpty(emailInput))
        {
            if (ValidationHelper.IsValidEmail(emailInput))
            {
                email = emailInput;
            }
            else
            {
                UIHelper.ShowError("Email không hợp lệ! Giữ nguyên email cũ.");
            }
        }
        string newPassword = null;
        string changePassword = UIHelper.GetInput("Có muốn đổi mật khẩu? (Y/N)");
        if (changePassword.ToUpper() == "Y")
        {
            newPassword = ConsoleHelper.GetValidPassword("Mật khẩu mới: ");
        }

        bool success = await _customerService.UpdateWithPasswordAsync(customerId, username, fullName, phone, email, newPassword);

        if (success)
        {
            UIHelper.ShowSuccess("Cập nhật thông tin Khách hàng thành công!");
        }
        else
        {
            UIHelper.ShowError("Cập nhật thông tin Khách hàng thất bại!");
        }
    }
    catch (Exception ex)
    {
        UIHelper.ShowError($"Lỗi khi cập nhật Khách hàng: {ex.Message}");
    }

    UIHelper.WaitForAnyKey();
}

        private async Task DeleteCustomerAsync()
        {
            UIHelper.DrawHeader("XÓA KHÁCH HÀNG");

            try
            {
                string idInput = UIHelper.GetInput("Nhập ID Khách hàng cần xóa");
                if (!int.TryParse(idInput, out int customerId))
                {
                    UIHelper.ShowError("ID không hợp lệ!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var customer = await _customerService.GetByIdAsync(customerId);

                if (customer == null)
                {
                    UIHelper.ShowError($"Không tìm thấy Khách hàng với ID: {customerId}");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                Console.WriteLine($"\nThông tin Khách hàng sẽ bị xóa:");
                Console.WriteLine($"ID: {customer.CustomerId}");
                Console.WriteLine($"Tên đăng nhập: {customer.Username}");
                Console.WriteLine($"Họ tên: {customer.FullName}");
                Console.WriteLine($"Email: {customer.Email}");

                UIHelper.ShowWarning("CẢNH BÁO: Việc xóa Khách hàng sẽ đánh dấu xóa (soft delete)!");
                
                string confirm = UIHelper.GetInput($"Bạn có chắc chắn muốn xóa Khách hàng: {customer.FullName} (ID: {customer.CustomerId})? (Y/N)");
                
                if (confirm.ToUpper() == "Y")
                {
                    bool success = await _customerService.DeleteAsync(customerId);

                    if (success)
                    {
                        UIHelper.ShowSuccess("Xóa Khách hàng thành công!");
                    }
                    else
                    {
                        UIHelper.ShowError("Xóa Khách hàng thất bại!");
                    }
                }
                else
                {
                    UIHelper.ShowInfo("Hủy xóa Khách hàng.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi xóa Khách hàng: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        // =================== QUẢN LÝ DỊCH VỤ ===================
        private async Task ManageServiceAsync()
        {
            while (true)
            {
                UIHelper.DrawHeader("QUẢN LÝ DỊCH VỤ");

                string[] menuItems = {
                    "Xem danh sách Dịch vụ",
                    "Thêm Dịch vụ mới",
                    "Cập nhật Dịch vụ",
                    "Xóa Dịch vụ"
                };

                UIHelper.DrawMenu(menuItems, "QUẢN LÝ DỊCH VỤ");

                string choice = UIHelper.GetInput("Lựa chọn của bạn");
                switch (choice)
                {
                    case "1":
                        await ViewServicesAsync();
                        break;
                    case "2":
                        await AddServiceAsync();
                        break;
                    case "3":
                        await UpdateServiceAsync();
                        break;
                    case "4":
                        await DeleteServiceAsync();
                        break;
                    case "0":
                        return;
                    default:
                        UIHelper.ShowError("Lựa chọn không hợp lệ!");
                        UIHelper.WaitForAnyKey();
                        break;
                }
            }
        }

        private async Task ViewServicesAsync()
        {
            try
            {
                UIHelper.DrawHeader("DANH SÁCH DỊCH VỤ");
                var services = await _serviceService.GetAllAsync();

                if (!services.Any())
                {
                    UIHelper.ShowWarning("Không có Dịch vụ nào trong hệ thống!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var headers = new[] { "ID", "Tên dịch vụ", "Mô tả", "Giá (VND)", "Thời gian (phút)" };
                var rows = services.Select(s => new[]
                {
                    s.ServiceId.ToString(),
                    s.Name,
                    s.Description.Length > 20 ? s.Description.Substring(0, 20) + "..." : s.Description,
                    $"{s.Price:N0}",
                    s.DurationMinutes.ToString()
                }).ToArray();

                UIHelper.DrawTable(headers, rows);
                UIHelper.ShowInfo($"Tổng số Dịch vụ: {services.Count()}");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải danh sách Dịch vụ: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task AddServiceAsync()
        {
            UIHelper.DrawHeader("THÊM DỊCH VỤ MỚI");

            try
            {
                string name = UIHelper.GetInput("Tên dịch vụ");
                if (string.IsNullOrWhiteSpace(name))
                {
                    UIHelper.ShowError("Tên dịch vụ không được để trống!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                string description = UIHelper.GetInput("Mô tả dịch vụ");
                if (string.IsNullOrWhiteSpace(description))
                {
                    UIHelper.ShowError("Mô tả dịch vụ không được để trống!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                decimal price = ValidationHelper.GetValidDecimal("Giá dịch vụ (VND): ");
                int durationMinutes = ValidationHelper.GetValidInt("Thời gian thực hiện (phút): ", 1);

                int serviceId = await _serviceService.CreateAsync(name, description, price, durationMinutes);

                if (serviceId > 0)
                {
                    UIHelper.ShowSuccess($"Thêm Dịch vụ thành công với ID: {serviceId}");
                }
                else
                {
                    UIHelper.ShowError("Thêm Dịch vụ thất bại!");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi thêm Dịch vụ: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task UpdateServiceAsync()
        {
            UIHelper.DrawHeader("CẬP NHẬT THÔNG TIN DỊCH VỤ");

            try
            {
                string idInput = UIHelper.GetInput("Nhập ID Dịch vụ cần cập nhật");
                if (!int.TryParse(idInput, out int serviceId))
                {
                    UIHelper.ShowError("ID không hợp lệ!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var service = await _serviceService.GetByIdAsync(serviceId);

                if (service == null)
                {
                    UIHelper.ShowError($"Không tìm thấy Dịch vụ với ID: {serviceId}");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                Console.WriteLine($"\nThông tin hiện tại:");
                Console.WriteLine($"Tên: {service.Name}");
                Console.WriteLine($"Mô tả: {service.Description}");
                Console.WriteLine($"Giá: {service.Price:N0} VND");
                Console.WriteLine($"Thời gian: {service.DurationMinutes} phút");

                Console.WriteLine("\n--- Nhập thông tin mới (Enter để giữ nguyên) ---");

                string name = UIHelper.GetInput("Tên dịch vụ mới");
                name = string.IsNullOrEmpty(name) ? service.Name : name;

                string description = UIHelper.GetInput("Mô tả mới");
                description = string.IsNullOrEmpty(description) ? service.Description : description;

                Console.Write("Giá mới (VND): ");
                string priceInput = Console.ReadLine()?.Trim();
                decimal price = service.Price;
                if (!string.IsNullOrEmpty(priceInput) && decimal.TryParse(priceInput, out decimal newPrice) && newPrice > 0)
                {
                    price = newPrice;
                }

                Console.Write("Thời gian mới (phút): ");
                string durationInput = Console.ReadLine()?.Trim();
                int durationMinutes = service.DurationMinutes;
                if (!string.IsNullOrEmpty(durationInput) && int.TryParse(durationInput, out int newDuration) && newDuration > 0)
                {
                    durationMinutes = newDuration;
                }

                bool success = await _serviceService.UpdateAsync(serviceId, name, description, price, durationMinutes);

                if (success)
                {
                    UIHelper.ShowSuccess("Cập nhật thông tin Dịch vụ thành công!");
                }
                else
                {
                    UIHelper.ShowError("Cập nhật thông tin Dịch vụ thất bại!");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi cập nhật Dịch vụ: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task DeleteServiceAsync()
        {
            UIHelper.DrawHeader("XÓA DỊCH VỤ");

            try
            {
                string idInput = UIHelper.GetInput("Nhập ID Dịch vụ cần xóa");
                if (!int.TryParse(idInput, out int serviceId))
                {
                    UIHelper.ShowError("ID không hợp lệ!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var service = await _serviceService.GetByIdAsync(serviceId);

                if (service == null)
                {
                    UIHelper.ShowError($"Không tìm thấy Dịch vụ với ID: {serviceId}");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                Console.WriteLine($"\nThông tin Dịch vụ sẽ bị xóa:");
                Console.WriteLine($"ID: {service.ServiceId}");
                Console.WriteLine($"Tên: {service.Name}");
                Console.WriteLine($"Mô tả: {service.Description}");
                Console.WriteLine($"Giá: {service.Price:N0} VND");

                UIHelper.ShowWarning("CẢNH BÁO: Việc xóa Dịch vụ sẽ đánh dấu xóa (soft delete)!");
                
                string confirm = UIHelper.GetInput($"Bạn có chắc chắn muốn xóa Dịch vụ: {service.Name} (ID: {service.ServiceId})? (Y/N)");
                
                if (confirm.ToUpper() == "Y")
                {
                    bool success = await _serviceService.DeleteAsync(serviceId);

                    if (success)
                    {
                        UIHelper.ShowSuccess("Xóa Dịch vụ thành công!");
                    }
                    else
                    {
                        UIHelper.ShowError("Xóa Dịch vụ thất bại!");
                    }
                }
                else
                {
                    UIHelper.ShowInfo("Hủy xóa Dịch vụ.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi xóa Dịch vụ: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        static void ManageBookings()
{
    while(true)
    {
        Console.Clear();
        Console.WriteLine("=== MANAGE BOOKINGS (ADMIN) ===");
        Console.WriteLine("1. View All Bookings");
        Console.WriteLine("2. Update by Customer");
        Console.WriteLine("3. Update by Barber");
        Console.WriteLine("4. Update by Service");
        Console.WriteLine("5. Update by Booking ID");
        Console.WriteLine("0. Back to Admin Menu");
        Console.Write("Choose: ");
        var ch = Console.ReadLine();
        if (ch=="0") return;
        switch(ch)
        {
            case "1":
                var list = _adminService.GetAllBookingsAsync().Result;
                // in danh sách giống ViewAllBookings()
                break;
            case "2":
                Console.Write("CustomerId: ");
                int cid = int.Parse(Console.ReadLine());
                Console.Write("New Status (Booked/Completed/Cancelled): ");
                string cs = Console.ReadLine();
                _adminService.UpdateBookingByCustomerAsync(cid,cs).Wait();
                break;
            case "3":
                Console.Write("BarberId: ");
                int bid = int.Parse(Console.ReadLine());
                Console.Write("New Status: ");
                string bs = Console.ReadLine();
                _adminService.UpdateBookingByBarberAsync(bid,bs).Wait();
                break;
            case "4":
                Console.Write("ServiceId: ");
                int sid = int.Parse(Console.ReadLine());
                Console.Write("New Status: ");
                string ss = Console.ReadLine();
                _adminService.UpdateBookingByServiceAsync(sid,ss).Wait();
                break;
            case "5":
                Console.Write("BookingId: ");
                int bogId = int.Parse(Console.ReadLine());
                Console.Write("New Status: ");
                string st = Console.ReadLine();
                _adminService.UpdateBookingByIdAsync(bogId,st).Wait();
                break;
            default:
                Console.WriteLine("Invalid choice."); Console.ReadKey();
                break;
        }
    }
}



        // =================== BÁO CÁO THỐNG KÊ ===================
        private async Task ViewReportsAsync()
        {
            while (true)
            {
                UIHelper.DrawHeader("BÁO CÁO THỐNG KÊ");

                string[] menuItems = {
                    "Thống kê tổng quan",
                    "Báo cáo booking theo ngày",
                    "Báo cáo doanh thu theo tháng",
                    "Top Barber được book nhiều nhất",
                    "Top Dịch vụ phổ biến"
                };

                UIHelper.DrawMenu(menuItems, "BÁO CÁO");

                string choice = UIHelper.GetInput("Lựa chọn của bạn");
                switch (choice)
                {
                    case "1":
                        await ViewOverallStatsAsync();
                        break;
                    case "2":
                        await ViewDailyBookingReportAsync();
                        break;
                    case "3":
                        await ViewMonthlyRevenueReportAsync();
                        break;
                    case "4":
                        await ViewTopBarbersAsync();
                        break;
                    case "5":
                        await ViewTopServicesAsync();
                        break;
                    case "0":
                        return;
                    default:
                        UIHelper.ShowError("Lựa chọn không hợp lệ!");
                        UIHelper.WaitForAnyKey();
                        break;
                }
            }
        }

        private async Task ViewOverallStatsAsync()
        {
            UIHelper.DrawHeader("THỐNG KÊ TỔNG QUAN");

            try
            {
                using var connection = DBHelper.GetConnection();
                using var command = new MySqlCommand("sp_report_dashboard_stats", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    // ✅ SỬA: Sử dụng indexer thay vì GetInt32(string)
                    var totalBarbers = Convert.ToInt32(reader["TotalBarbers"]);
                    var totalCustomers = Convert.ToInt32(reader["TotalCustomers"]);
                    var totalServices = Convert.ToInt32(reader["TotalServices"]);
                    var totalBookings = Convert.ToInt32(reader["TotalBookings"]);
                    var todayBookings = Convert.ToInt32(reader["TodayBookings"]);
                    var completedBookings = Convert.ToInt32(reader["CompletedBookings"]);
                    var cancelledBookings = Convert.ToInt32(reader["CancelledBookings"]);
                    var totalRevenue = Convert.ToDecimal(reader["TotalRevenue"]);

                    Console.WriteLine("========================================");
                    Console.WriteLine("           THỐNG KÊ HỆ THỐNG           ");
                    Console.WriteLine("========================================");
                    Console.WriteLine($"Tổng số Barber:      {totalBarbers,10}");
                    Console.WriteLine($"Tổng số Khách hàng:  {totalCustomers,10}");
                    Console.WriteLine($"Tổng số Dịch vụ:     {totalServices,10}");
                    Console.WriteLine($"Tổng số Booking:     {totalBookings,10}");
                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine($"Booking hôm nay:     {todayBookings,10}");
                    Console.WriteLine($"Booking hoàn thành:  {completedBookings,10}");
                    Console.WriteLine($"Booking hủy:         {cancelledBookings,10}");
                    Console.WriteLine($"Tổng doanh thu:      {totalRevenue:N0} VND");
                    Console.WriteLine("========================================");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải thống kê: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task ViewDailyBookingReportAsync()
        {
            UIHelper.DrawHeader("BÁO CÁO BOOKING THEO NGÀY");

            try
            {
                string dateInput = UIHelper.GetInput("Nhập ngày cần xem (dd/MM/yyyy) - Enter để xem hôm nay");
                DateTime targetDate = DateTime.Today;

                if (!string.IsNullOrEmpty(dateInput))
                {
                    if (!DateTime.TryParseExact(dateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out targetDate))
                    {
                        UIHelper.ShowError("Định dạng ngày không hợp lệ!");
                        UIHelper.WaitForAnyKey();
                        return;
                    }
                }

                using var connection = DBHelper.GetConnection();
                using var command = new MySqlCommand("sp_report_daily_bookings", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                
                command.Parameters.AddWithValue("@p_date", targetDate.Date);

                using var reader = await command.ExecuteReaderAsync();
                
                var headers = new[] { "ID", "Khách hàng", "Barber", "Dịch vụ", "Giờ", "Trạng thái" };
                var rows = new List<string[]>();

                while (await reader.ReadAsync())
                {
                    // ✅ SỬA: Sử dụng indexer và ToString()
                    rows.Add(new[]
                    {
                        reader["BookingId"].ToString(),
                        reader["CustomerName"]?.ToString() ?? "N/A",
                        reader["BarberName"]?.ToString() ?? "N/A",
                        reader["ServiceName"]?.ToString() ?? "N/A",
                        $"{reader["StartTime"]} - {reader["EndTime"]}",
                        reader["Status"].ToString()
                    });
                }

                if (rows.Any())
                {
                    UIHelper.DrawTable(headers, rows.ToArray());
                }
                else
                {
                    UIHelper.ShowInfo($"Không có booking nào trong ngày {targetDate:dd/MM/yyyy}");
                }

                // Đọc result set thứ 2 (summary)
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    var completed = Convert.ToInt32(reader["CompletedBookings"]);
                    var booked = Convert.ToInt32(reader["BookedBookings"]);
                    var cancelled = Convert.ToInt32(reader["CancelledBookings"]);
                    var total = Convert.ToInt32(reader["TotalBookings"]);

                    Console.WriteLine("\n=== TỔNG KẾT ===");
                    Console.WriteLine($"Hoàn thành: {completed} | Đã đặt: {booked} | Hủy: {cancelled} | Tổng: {total}");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải báo cáo: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task ViewMonthlyRevenueReportAsync()
        {
            UIHelper.DrawHeader("BÁO CÁO DOANH THU THEO THÁNG");

            try
            {
                string monthInput = UIHelper.GetInput("Nhập tháng/năm (MM/yyyy) - Enter để xem tháng này");
                DateTime targetMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                if (!string.IsNullOrEmpty(monthInput))
                {
                    if (!DateTime.TryParseExact($"01/{monthInput}", "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out targetMonth))
                    {
                        UIHelper.ShowError("Định dạng tháng/năm không hợp lệ!");
                        UIHelper.WaitForAnyKey();
                        return;
                    }
                }

                using var connection = DBHelper.GetConnection();
                using var command = new MySqlCommand("sp_report_monthly_revenue", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                
                command.Parameters.AddWithValue("@p_year", targetMonth.Year);
                command.Parameters.AddWithValue("@p_month", targetMonth.Month);

                using var reader = await command.ExecuteReaderAsync();
                
                var headers = new[] { "Ngày", "Số booking", "Doanh thu (VND)" };
                var rows = new List<string[]>();

                while (await reader.ReadAsync())
                {
                    // ✅ SỬA: Sử dụng indexer
                    var date = Convert.ToDateTime(reader["BookingDate"]);
                    var bookingCount = Convert.ToInt32(reader["TotalBookings"]);
                    var revenue = Convert.ToDecimal(reader["Revenue"]);

                    rows.Add(new[]
                    {
                        date.ToString("dd/MM"),
                        bookingCount.ToString(),
                        $"{revenue:N0}"
                    });
                }

                if (rows.Any())
                {
                    UIHelper.DrawTable(headers, rows.ToArray());
                }
                else
                {
                    UIHelper.ShowInfo($"Không có booking hoàn thành nào trong tháng {targetMonth:MM/yyyy}");
                }

                // Đọc result set thứ 2 (summary)
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    var totalRevenue = Convert.ToDecimal(reader["TotalRevenue"]);
                    var totalBookings = Convert.ToInt32(reader["TotalBookings"]);

                    Console.WriteLine("========================================");
                    Console.WriteLine($"TỔNG DOANH THU THÁNG {targetMonth:MM/yyyy}");
                    Console.WriteLine($"Tổng booking: {totalBookings}");
                    Console.WriteLine($"Tổng doanh thu: {totalRevenue:N0} VND");
                    Console.WriteLine("========================================");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải báo cáo doanh thu: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task ViewTopBarbersAsync()
        {
            UIHelper.DrawHeader("TOP BARBER ĐƯỢC BOOK NHIỀU NHẤT");

            try
            {
                using var connection = DBHelper.GetConnection();
                using var command = new MySqlCommand("sp_report_top_barbers", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                
                command.Parameters.AddWithValue("@p_limit", 10);

                using var reader = await command.ExecuteReaderAsync();
                
                var headers = new[] { "Thứ hạng", "Tên Barber", "Số booking", "Doanh thu (VND)" };
                var rows = new List<string[]>();

                int rank = 1;
                while (await reader.ReadAsync())
                {
                    // ✅ SỬA: Sử dụng indexer
                    var barberName = reader["BarberName"].ToString();
                    var bookingCount = Convert.ToInt32(reader["BookingCount"]);
                    var revenue = Convert.ToDecimal(reader["Revenue"]);

                    rows.Add(new[]
                    {
                        rank.ToString(),
                        barberName,
                        bookingCount.ToString(),
                        $"{revenue:N0}"
                    });
                    rank++;
                }

                if (rows.Any())
                {
                    UIHelper.DrawTable(headers, rows.ToArray());
                }
                else
                {
                    UIHelper.ShowInfo("Chưa có dữ liệu booking hoàn thành");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải thống kê Barber: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task ViewTopServicesAsync()
        {
            UIHelper.DrawHeader("TOP DỊCH VỤ PHỔ BIẾN");

            try
            {
                using var connection = DBHelper.GetConnection();
                using var command = new MySqlCommand("sp_report_top_services", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                
                command.Parameters.AddWithValue("@p_limit", 10);

                using var reader = await command.ExecuteReaderAsync();
                
                var headers = new[] { "Thứ hạng", "Tên dịch vụ", "Số lần book", "Doanh thu (VND)" };
                var rows = new List<string[]>();

                int rank = 1;
                while (await reader.ReadAsync())
                {
                    // ✅ SỬA: Sử dụng indexer
                    var serviceName = reader["ServiceName"].ToString();
                    var bookingCount = Convert.ToInt32(reader["BookingCount"]);
                    var revenue = Convert.ToDecimal(reader["Revenue"]);

                    rows.Add(new[]
                    {
                        rank.ToString(),
                        serviceName,
                        bookingCount.ToString(),
                        $"{revenue:N0}"
                    });
                    rank++;
                }

                if (rows.Any())
                {
                    UIHelper.DrawTable(headers, rows.ToArray());
                }
                else
                {
                    UIHelper.ShowInfo("Chưa có dữ liệu booking hoàn thành");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải thống kê Dịch vụ: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private string GetDayName(int dayOfWeek)
        {
            return dayOfWeek switch
            {
                1 => "Chủ Nhật",
                2 => "Thứ Hai",
                3 => "Thứ Ba",
                4 => "Thứ Tư",
                5 => "Thứ Năm",
                6 => "Thứ Sáu",
                7 => "Thứ Bảy",
                _ => "Không xác định"
            };
        }
    }
}
