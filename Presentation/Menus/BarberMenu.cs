using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using BarberShopManagement.Business.Interfaces;
using BarberShopManagement.Models;
using BarberShopManagement.Presentation.Utils;

namespace BarberShopManagement.Presentation.Menus
{
    public class BarberMenu
    {
        private readonly IBarberService _barberService;
        private readonly IBookingService _bookingService;
        private readonly IServiceService _serviceService;

        private Barber? _currentBarber;

        public BarberMenu(
            IBarberService barberService,
            IBookingService bookingService,
            IServiceService serviceService)
        {
            _barberService = barberService;
            _bookingService = bookingService;
            _serviceService = serviceService;
        }

        public async Task ShowLoginAsync()
        {
            UIHelper.DrawHeader("ĐĂNG NHẬP BARBER");

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

                _currentBarber = await _barberService.AuthenticateAsync(username, password);

                if (_currentBarber != null)
                {
                    UIHelper.ShowSuccess($"Đăng nhập thành công! Xin chào, {_currentBarber.FullName}");
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
                UIHelper.DrawHeader($"PANEL BARBER - {_currentBarber!.FullName}");

                string[] menuItems = {
                    "Xem lịch hôm nay",
                    "Xem lịch tuần này",
                    "Xem lịch theo ngày",
                    "Cập nhật thông tin cá nhân",
                    "Xem thống kê cá nhân"
                };

                UIHelper.DrawMenu(menuItems, "MENU BARBER");

                string choice = UIHelper.GetInput("Lựa chọn của bạn");
                switch (choice)
                {
                    case "1":
                        await ViewTodayScheduleAsync();
                        break;
                    case "2":
                        await ViewWeekScheduleAsync();
                        break;
                    case "3":
                        await ViewScheduleByDateAsync();
                        break;
                    case "4":
                        await UpdateProfileAsync();
                        break;
                    case "0":
                        _currentBarber = null;
                        return;
                    default:
                        UIHelper.ShowError("Lựa chọn không hợp lệ!");
                        UIHelper.WaitForAnyKey();
                        break;
                }
            }
        }

        private async Task ViewTodayScheduleAsync()
        {
            UIHelper.DrawHeader($"LỊCH HÔM NAY ({DateTime.Now:dd/MM/yyyy})");

            try
            {
                var bookings = (await _bookingService.GetByBarberIdAsync(_currentBarber!.BarberId))
                    .Where(b => b.StartTime.Date == DateTime.Now.Date)
                    .OrderBy(b => b.StartTime);

                if (!bookings.Any())
                {
                    UIHelper.ShowInfo("Không có lịch hẹn nào hôm nay!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var headers = new[] { "ID", "Giờ", "Khách hàng", "Dịch vụ", "Trạng thái" };
                var rows = new List<string[]>();

                foreach (var booking in bookings)
                {
                    var service = await _serviceService.GetByIdAsync(booking.ServiceId);
                    string serviceName = service?.Name ?? "Không xác định";
                    if (serviceName.Length > 15) serviceName = serviceName.Substring(0, 15) + "...";

                    rows.Add(new[]
                    {
                        booking.BookingId.ToString(),
                        $"{booking.StartTime:HH:mm} - {booking.EndTime:HH:mm}",
                        "Khách hàng", // Sẽ được cập nhật khi có CustomerName
                        serviceName,
                        booking.Status
                    });
                }

                UIHelper.DrawTable(headers, rows.ToArray());
                UIHelper.ShowInfo($"Tổng số lịch hẹn: {bookings.Count()}");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải lịch: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task ViewWeekScheduleAsync()
        {
            try
            {
                DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                DateTime endOfWeek = startOfWeek.AddDays(7);

                UIHelper.DrawHeader($"LỊCH TUẦN ({startOfWeek:dd/MM} - {endOfWeek:dd/MM})");

                var bookings = (await _bookingService.GetByBarberIdAsync(_currentBarber!.BarberId))
                    .Where(b => b.StartTime >= startOfWeek && b.StartTime <= endOfWeek)
                    .OrderBy(b => b.StartTime);

                if (!bookings.Any())
                {
                    UIHelper.ShowInfo("Không có lịch hẹn nào tuần này!");
                }
                else
                {
                    var headers = new[] { "Ngày", "Thứ", "Giờ", "Dịch vụ", "Trạng thái" };
                    var rows = new List<string[]>();

                    foreach (var booking in bookings)
                    {
                        var service = await _serviceService.GetByIdAsync(booking.ServiceId);
                        var dayOfWeek = GetDayOfWeekName(booking.StartTime.DayOfWeek);
                        
                        rows.Add(new[]
                        {
                            booking.StartTime.ToString("dd/MM"),
                            dayOfWeek,
                            $"{booking.StartTime:HH:mm} - {booking.EndTime:HH:mm}",
                            service?.Name ?? "Không xác định",
                            booking.Status
                        });
                    }

                    UIHelper.DrawTable(headers, rows.ToArray());
                    UIHelper.ShowInfo($"Tổng số lịch hẹn tuần này: {bookings.Count()}");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải lịch tuần: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task ViewScheduleByDateAsync()
        {
            UIHelper.DrawHeader("XEM LỊCH THEO NGÀY");

            try
            {
                string dateInput = UIHelper.GetInput("Nhập ngày cần xem (dd/MM/yyyy)");
                if (!DateTime.TryParseExact(dateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime targetDate))
                {
                    UIHelper.ShowError("Định dạng ngày không hợp lệ!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var bookings = (await _bookingService.GetByBarberIdAsync(_currentBarber!.BarberId))
                    .Where(b => b.StartTime.Date == targetDate.Date)
                    .OrderBy(b => b.StartTime);

                UIHelper.DrawHeader($"LỊCH NGÀY {targetDate:dd/MM/yyyy} ({GetDayOfWeekName(targetDate.DayOfWeek)})");

                if (!bookings.Any())
                {
                    UIHelper.ShowInfo($"Không có lịch hẹn nào trong ngày {targetDate:dd/MM/yyyy}!");
                }
                else
                {
                    var headers = new[] { "ID", "Giờ", "Dịch vụ", "Giá", "Trạng thái" };
                    var rows = new List<string[]>();

                    decimal totalRevenue = 0;
                    foreach (var booking in bookings)
                    {
                        var service = await _serviceService.GetByIdAsync(booking.ServiceId);
                        if (service != null && booking.Status == "Completed")
                        {
                            totalRevenue += service.Price;
                        }

                        rows.Add(new[]
                        {
                            booking.BookingId.ToString(),
                            $"{booking.StartTime:HH:mm} - {booking.EndTime:HH:mm}",
                            service?.Name ?? "Không xác định",
                            service != null ? $"{service.Price:N0}" : "0",
                            booking.Status
                        });
                    }

                    UIHelper.DrawTable(headers, rows.ToArray());
                    
                    Console.WriteLine($"\nTổng kết ngày {targetDate:dd/MM/yyyy}:");
                    Console.WriteLine($"- Tổng lịch hẹn: {bookings.Count()}");
                    Console.WriteLine($"- Hoàn thành: {bookings.Count(b => b.Status == "Completed")}");
                    Console.WriteLine($"- Doanh thu: {totalRevenue:N0} VND");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải lịch: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task UpdateProfileAsync()
        {
            UIHelper.DrawHeader("CẬP NHẬT THÔNG TIN CÁ NHÂN");

            try
            {
                Console.WriteLine("Thông tin hiện tại:");
                Console.WriteLine($"Họ tên: {_currentBarber!.FullName}");
                Console.WriteLine($"Số điện thoại: {_currentBarber.Phone}");

                string currentDayOff = _currentBarber.OffDayOfWeek switch
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
                Console.WriteLine($"Ngày nghỉ: {currentDayOff}");

                Console.WriteLine("\n--- Nhập thông tin mới (Enter để giữ nguyên) ---");

                string fullName = UIHelper.GetInput("Họ tên mới");
                fullName = string.IsNullOrEmpty(fullName) ? _currentBarber.FullName : fullName;

                Console.Write("Số điện thoại mới: ");
                string phoneInput = Console.ReadLine()?.Trim();
                string phone = _currentBarber.Phone;
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

                Console.WriteLine("\nNgày nghỉ:");
                Console.WriteLine("1. Chủ Nhật");
                Console.WriteLine("2. Thứ Hai");
                Console.WriteLine("3. Thứ Ba");
                Console.WriteLine("4. Thứ Tư");
                Console.WriteLine("5. Thứ Năm");
                Console.WriteLine("6. Thứ Sáu");
                Console.WriteLine("7. Thứ Bảy");
                
                string offDayInput = UIHelper.GetInput("Chọn ngày nghỉ mới (Enter để giữ nguyên)");
                int offDayOfWeek = _currentBarber.OffDayOfWeek;
                if (!string.IsNullOrEmpty(offDayInput) && int.TryParse(offDayInput, out int newOffDay) && newOffDay >= 1 && newOffDay <= 7)
                {
                    offDayOfWeek = newOffDay;
                }

                bool success = await _barberService.UpdateAsync(
                    _currentBarber.BarberId,
                    _currentBarber.Username,
                    fullName,
                    phone,
                    offDayOfWeek
                );

                if (success)
                {
                    _currentBarber = await _barberService.GetByIdAsync(_currentBarber.BarberId);
                    UIHelper.ShowSuccess("Cập nhật thông tin thành công!");
                }
                else
                {
                    UIHelper.ShowError("Cập nhật thông tin thất bại!");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi cập nhật thông tin: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task ViewPersonalStatsAsync()
        {
            UIHelper.DrawHeader("THỐNG KÊ CÁ NHÂN");

            try
            {
                var allBookings = await _bookingService.GetByBarberIdAsync(_currentBarber!.BarberId);
                
                // Thống kê tổng quan
                var totalBookings = allBookings.Count();
                var completedBookings = allBookings.Count(b => b.Status == "Completed");
                var cancelledBookings = allBookings.Count(b => b.Status == "Cancelled");
                var todayBookings = allBookings.Count(b => b.StartTime.Date == DateTime.Today);

                // Thống kê theo tháng hiện tại
                var thisMonthBookings = allBookings.Where(b => 
                    b.StartTime.Month == DateTime.Now.Month && 
                    b.StartTime.Year == DateTime.Now.Year);

                decimal thisMonthRevenue = 0;
                foreach (var booking in thisMonthBookings.Where(b => b.Status == "Completed"))
                {
                    var service = await _serviceService.GetByIdAsync(booking.ServiceId);
                    if (service != null)
                    {
                        thisMonthRevenue += service.Price;
                    }
                }

                // Dịch vụ phổ biến nhất
                var serviceStats = new Dictionary<int, int>();
                foreach (var booking in allBookings.Where(b => b.Status == "Completed"))
                {
                    if (serviceStats.ContainsKey(booking.ServiceId))
                        serviceStats[booking.ServiceId]++;
                    else
                        serviceStats[booking.ServiceId] = 1;
                }

                Console.WriteLine("========================================");
                Console.WriteLine("           THỐNG KÊ CÁ NHÂN            ");
                Console.WriteLine("========================================");
                Console.WriteLine($"Tổng booking:        {totalBookings,10}");
                Console.WriteLine($"Hoàn thành:          {completedBookings,10}");
                Console.WriteLine($"Bị hủy:              {cancelledBookings,10}");
                Console.WriteLine($"Booking hôm nay:     {todayBookings,10}");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine($"Booking tháng này:   {thisMonthBookings.Count(),10}");
                Console.WriteLine($"Doanh thu tháng này: {thisMonthRevenue:N0} VND");
                Console.WriteLine("========================================");

                if (serviceStats.Any())
                {
                    Console.WriteLine("\nDịch vụ được book nhiều nhất:");
                    var topServices = serviceStats.OrderByDescending(s => s.Value).Take(3);
                    int rank = 1;
                    foreach (var serviceStat in topServices)
                    {
                        var service = await _serviceService.GetByIdAsync(serviceStat.Key);
                        Console.WriteLine($"{rank}. {service?.Name ?? "Không xác định"}: {serviceStat.Value} lần");
                        rank++;
                    }
                }

                // Thống kê theo ngày trong tuần
                Console.WriteLine("\nThống kê theo ngày trong tuần:");
                var dayStats = allBookings
                    .Where(b => b.Status == "Completed")
                    .GroupBy(b => b.StartTime.DayOfWeek)
                    .OrderBy(g => g.Key);

                foreach (var dayStat in dayStats)
                {
                    var dayName = GetDayOfWeekName(dayStat.Key);
                    Console.WriteLine($"{dayName}: {dayStat.Count()} booking");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải thống kê: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private string GetDayOfWeekName(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => "Chủ Nhật",
                DayOfWeek.Monday => "Thứ Hai",
                DayOfWeek.Tuesday => "Thứ Ba",
                DayOfWeek.Wednesday => "Thứ Tư",
                DayOfWeek.Thursday => "Thứ Năm",
                DayOfWeek.Friday => "Thứ Sáu",
                DayOfWeek.Saturday => "Thứ Bảy",
                _ => "Không xác định"
            };
        }
    }
}
