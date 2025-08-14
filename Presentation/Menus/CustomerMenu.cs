using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BarberShopManagement.Business.Interfaces;
using BarberShopManagement.Models;
using BarberShopManagement.Presentation.Utils;

namespace BarberShopManagement.Presentation.Menus
{
    public class CustomerMenu
    {
        private readonly ICustomerService _customerService;
        private readonly IBarberService _barberService;
        private readonly IServiceService _serviceService;
        private readonly IBookingService _bookingService;

        private Customer? _currentCustomer;

        public CustomerMenu(
            ICustomerService customerService,
            IBarberService barberService,
            IServiceService serviceService,
            IBookingService bookingService)
        {
            _customerService = customerService;
            _barberService = barberService;
            _serviceService = serviceService;
            _bookingService = bookingService;
        }

        public async Task ShowLoginAsync()
        {
            UIHelper.DrawHeader("ĐĂNG NHẬP KHÁCH HÀNG");

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

                _currentCustomer = await _customerService.AuthenticateAsync(username, password);

                if (_currentCustomer != null)
                {
                    UIHelper.ShowSuccess($"Đăng nhập thành công! Xin chào, {_currentCustomer.FullName}");
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

        // ✅ THAY THẾ method ShowRegisterAsync() trong CustomerMenu.cs

public async Task ShowRegisterAsync()
{
    UIHelper.DrawHeader("ĐĂNG KÝ TÀI KHOẢN");

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
            UIHelper.ShowSuccess("Đăng ký thành công! Vui lòng đăng nhập để tiếp tục.");
        }
        else
        {
            // ✅ SỬA: Thông báo lỗi chi tiết
            switch (result)
            {
                case -1:
                    UIHelper.ShowError("Tên đăng nhập đã tồn tại! Vui lòng chọn tên khác.");
                    break;
                case -2:
                    UIHelper.ShowError("Số điện thoại đã tồn tại! Vui lòng sử dụng số khác.");
                    break;
                case -3:
                    UIHelper.ShowError("Email đã tồn tại! Vui lòng sử dụng email khác.");
                    break;
                default:
                    UIHelper.ShowError("Đăng ký thất bại! Vui lòng thử lại.");
                    break;
            }
        }
    }
    catch (Exception ex)
    {
        UIHelper.ShowError($"Lỗi đăng ký: {ex.Message}");
    }

    UIHelper.WaitForAnyKey();
}


        private async Task ShowMenuAsync()
        {
            while (true)
            {
                UIHelper.DrawHeader($"PANEL KHÁCH HÀNG - {_currentCustomer!.FullName}");

                string[] menuItems = {
                    "Xem danh sách dịch vụ",
                    "Đặt lịch hẹn",
                    "Xem lịch hẹn của tôi",
                    "Hủy lịch hẹn",
                    "Lịch sử booking",
                    "Cập nhật thông tin cá nhân"
                };

                UIHelper.DrawMenu(menuItems, "MENU KHÁCH HÀNG");

                string choice = UIHelper.GetInput("Lựa chọn của bạn");
                switch (choice)
                {
                    case "1":
                        await ViewServicesAsync();
                        break;
                    case "2":
                        await BookAppointmentAsync();
                        break;
                    case "3":
                        await ViewMyBookingsAsync();
                        break;
                    case "4":
                        await CancelBookingAsync();
                        break;
                    case "5":
                        await ViewBookingHistoryAsync();
                        break;
                    case "6":
                        await UpdateProfileAsync();
                        break;
                    case "0":
                        _currentCustomer = null;
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
            UIHelper.DrawHeader("DANH SÁCH DỊCH VỤ");

            try
            {
                var services = await _serviceService.GetAllAsync();

                if (!services.Any())
                {
                    UIHelper.ShowWarning("Không có dịch vụ nào!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var headers = new[] { "ID", "Tên dịch vụ", "Mô tả", "Giá (VND)", "Thời gian (phút)" };
                var rows = services.Select(s => new[]
                {
                    s.ServiceId.ToString(),
                    s.Name,
                    s.Description.Length > 30 ? s.Description.Substring(0, 30) + "..." : s.Description,
                    $"{s.Price:N0}",
                    s.DurationMinutes.ToString()
                }).ToArray();

                UIHelper.DrawTable(headers, rows);
                UIHelper.ShowInfo($"Tổng số dịch vụ: {services.Count()}");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải danh sách dịch vụ: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        // ✅ THAY THẾ method BookAppointmentAsync() trong CustomerMenu.cs

private async Task BookAppointmentAsync()
{
    UIHelper.DrawHeader("ĐẶT LỊCH HẸN");

    try
    {
        // Bước 1: Chọn nhiều dịch vụ
        var services = await _serviceService.GetAllAsync();
        var selectedServices = await SelectMultipleServicesAsync(services);
        if (selectedServices == null || !selectedServices.Any()) return;

        // Bước 2: Chọn ngày (giới hạn 30 ngày)
        var bookingDate = await SelectBookingDateWithin30DaysAsync();
        if (bookingDate == null) return;

        // Bước 3: Nhập giờ bắt đầu
        var startTime = await GetCustomStartTimeAsync(bookingDate.Value);
        if (startTime == null) return;

        // Tính thời gian kết thúc dựa trên tổng thời gian dịch vụ
        var totalDuration = selectedServices.Sum(s => s.DurationMinutes);
        var endTime = startTime.Value.AddMinutes(totalDuration);

        // Bước 4: Validate khung giờ làm việc
        if (!ValidationHelper.IsValidBookingTime(startTime.Value, endTime))
        {
            UIHelper.ShowError("Khung giờ không hợp lệ! Giờ làm việc: 8-12h và 14-18h");
            UIHelper.WaitForAnyKey();
            return;
        }

        // Bước 5: Kiểm tra customer đã book trong khung giờ này chưa
        if (await IsCustomerAlreadyBookedAsync(_currentCustomer!.CustomerId, startTime.Value, endTime))
        {
            UIHelper.ShowError("Bạn đã có lịch hẹn trong khung giờ này!");
            UIHelper.WaitForAnyKey();
            return;
        }

        // Bước 6: Chọn barber
        var availableBarbers = await _barberService.GetAvailableBarbersAsync(startTime.Value, endTime);
        var selectedBarber = await SelectBarber(availableBarbers);
        if (selectedBarber == null) return;

        // Bước 7: Xác nhận booking
        var totalPrice = selectedServices.Sum(s => s.Price);
        var serviceNames = string.Join(", ", selectedServices.Select(s => s.Name));

        if (await ConfirmMultiServiceBookingAsync(selectedBarber, selectedServices, startTime.Value, endTime, totalPrice, serviceNames))
        {
            UIHelper.ShowSuccess("Đặt lịch thành công!");
        }
    }
    catch (Exception ex)
    {
        UIHelper.ShowError($"Lỗi khi đặt lịch: {ex.Message}");
    }

    UIHelper.WaitForAnyKey();
}

// ✅ MỚI: Chọn nhiều dịch vụ
private async Task<List<Service>?> SelectMultipleServicesAsync(IEnumerable<Service> services)
{
    UIHelper.DrawHeader("CHỌN DỊCH VỤ (Có thể chọn nhiều, phân cách bằng dấu phẩy)");

    var serviceList = services.ToList();
    var headers = new[] { "ID", "Tên dịch vụ", "Giá (VND)", "Thời gian (phút)" };
    var rows = serviceList.Select(s => new[]
    {
        s.ServiceId.ToString(),
        s.Name,
        $"{s.Price:N0}",
        s.DurationMinutes.ToString()
    }).ToArray();

    UIHelper.DrawTable(headers, rows);

    while (true)
    {
        string input = UIHelper.GetInput("Nhập ID dịch vụ (phân cách bằng dấu phẩy, VD: 1,3,5)");
        
        if (input == "0") return null;

        var serviceIds = ValidationHelper.ParseServiceIds(input);
        
        if (serviceIds.Length == 0)
        {
            UIHelper.ShowError("Vui lòng nhập ít nhất 1 ID dịch vụ hợp lệ!");
            continue;
        }

        var selectedServices = serviceList.Where(s => serviceIds.Contains(s.ServiceId)).ToList();
        
        if (selectedServices.Count != serviceIds.Length)
        {
            UIHelper.ShowError("Một số ID dịch vụ không tồn tại!");
            continue;
        }

        // Hiển thị xác nhận
        Console.WriteLine("\nDịch vụ đã chọn:");
        var totalPrice = selectedServices.Sum(s => s.Price);
        var totalTime = selectedServices.Sum(s => s.DurationMinutes);
        
        foreach (var service in selectedServices)
        {
            Console.WriteLine($"- {service.Name}: {service.Price:N0} VND ({service.DurationMinutes} phút)");
        }
        Console.WriteLine($"Tổng: {totalPrice:N0} VND ({totalTime} phút)");

        string confirm = UIHelper.GetInput("Xác nhận chọn các dịch vụ này? (Y/N)");
        if (confirm.ToUpper() == "Y")
        {
            return selectedServices;
        }
    }
}

// ✅ MỚI: Chọn ngày trong 30 ngày
private async Task<DateTime?> SelectBookingDateWithin30DaysAsync()
{
    UIHelper.DrawHeader("CHỌN NGÀY ĐẶT LỊCH (Trong vòng 30 ngày)");

    Console.WriteLine($"Hôm nay: {DateTime.Today:dd/MM/yyyy}");
    Console.WriteLine($"Có thể đặt từ: {DateTime.Today:dd/MM/yyyy} đến {DateTime.Today.AddDays(30):dd/MM/yyyy}");

    while (true)
    {
        string dateInput = UIHelper.GetInput("Nhập ngày đặt lịch (dd/MM/yyyy)");
        
        if (dateInput == "0") return null;

        if (DateTime.TryParseExact(dateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
        {
            if (!ValidationHelper.IsWithin30Days(date))
            {
                UIHelper.ShowError("Chỉ có thể đặt lịch trong vòng 30 ngày tới!");
                continue;
            }

            return date;
        }
        
        UIHelper.ShowError("Định dạng ngày không hợp lệ! Vui lòng nhập theo định dạng dd/MM/yyyy");
    }
}

// ✅ MỚI: Nhập giờ bắt đầu tùy chỉnh
private async Task<DateTime?> GetCustomStartTimeAsync(DateTime date)
{
    UIHelper.DrawHeader($"NHẬP GIỜ BẮT ĐẦU - {date:dd/MM/yyyy}");
    
    Console.WriteLine("Khung giờ làm việc:");
    Console.WriteLine("- Buổi sáng: 08:00 - 12:00");
    Console.WriteLine("- Buổi chiều: 14:00 - 18:00");

    while (true)
    {
        string timeInput = UIHelper.GetInput("Nhập giờ bắt đầu (HH:mm, VD: 09:30)");
        
        if (timeInput == "0") return null;

        if (TimeSpan.TryParseExact(timeInput, @"hh\:mm", null, out TimeSpan time))
        {
            var startTime = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);
            
            // Validate trong khung giờ làm việc
            if ((time.Hours >= 8 && time.Hours < 12) || (time.Hours >= 14 && time.Hours < 18))
            {
                return startTime;
            }
            else
            {
                UIHelper.ShowError("Giờ bắt đầu phải trong khung 8-12h hoặc 14-18h!");
            }
        }
        else
        {
            UIHelper.ShowError("Định dạng giờ không hợp lệ! Vui lòng nhập theo định dạng HH:mm");
        }
    }
}

// ✅ MỚI: Kiểm tra customer đã book chưa
private async Task<bool> IsCustomerAlreadyBookedAsync(int customerId, DateTime startTime, DateTime endTime)
{
    var customerBookings = await _bookingService.GetByCustomerIdAsync(customerId);
    
    return customerBookings.Any(b => 
        b.Status == "Booked" && 
        ((startTime >= b.StartTime && startTime < b.EndTime) ||
         (endTime > b.StartTime && endTime <= b.EndTime) ||
         (startTime <= b.StartTime && endTime >= b.EndTime))
    );
}

// ✅ MỚI: Xác nhận booking nhiều dịch vụ
private async Task<bool> ConfirmMultiServiceBookingAsync(Barber barber, List<Service> services, DateTime startTime, DateTime endTime, decimal totalPrice, string serviceNames)
{
    UIHelper.DrawHeader("XÁC NHẬN ĐẶT LỊCH");

    Console.WriteLine($"Khách hàng: {_currentCustomer!.FullName}");
    Console.WriteLine($"Barber: {barber.FullName}");
    Console.WriteLine($"Dịch vụ: {serviceNames}");
    Console.WriteLine($"Ngày: {startTime:dd/MM/yyyy}");
    Console.WriteLine($"Giờ: {startTime:HH:mm} - {endTime:HH:mm}");
    Console.WriteLine($"Tổng thời gian: {services.Sum(s => s.DurationMinutes)} phút");
    Console.WriteLine($"Tổng giá: {totalPrice:N0} VND");

    string confirm = UIHelper.GetInput("Xác nhận đặt lịch này? (Y/N)");
    if (confirm.ToUpper() == "Y")
    {
        try
        {
            // Tạo booking cho từng dịch vụ
            bool allSuccess = true;
            var currentStart = startTime;
            
            foreach (var service in services)
            {
                var serviceEnd = currentStart.AddMinutes(service.DurationMinutes);
                
                int bookingId = await _bookingService.CreateAsync(
                    _currentCustomer.CustomerId,
                    barber.BarberId,
                    service.ServiceId,
                    currentStart,
                    serviceEnd
                );

                if (bookingId <= 0)
                {
                    allSuccess = false;
                    break;
                }
                
                currentStart = serviceEnd;
            }

            return allSuccess;
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Lỗi khi đặt lịch: {ex.Message}");
            return false;
        }
    }
    return false;
}





        private async Task<Service?> SelectService(IEnumerable<Service> services)
        {
            UIHelper.DrawHeader("CHỌN DỊCH VỤ");

            var serviceList = services.ToList();
            var headers = new[] { "STT", "Tên dịch vụ", "Giá (VND)", "Thời gian (phút)" };
            var rows = serviceList.Select((s, i) => new[]
            {
                (i + 1).ToString(),
                s.Name,
                $"{s.Price:N0}",
                s.DurationMinutes.ToString()
            }).ToArray();

            UIHelper.DrawTable(headers, rows);

            while (true)
            {
                string choiceInput = UIHelper.GetInput("Chọn dịch vụ (nhập số thứ tự)");
                if (int.TryParse(choiceInput, out int choice) && choice >= 1 && choice <= serviceList.Count)
                {
                    return serviceList[choice - 1];
                }
                
                if (choiceInput == "0")
                    return null;

                UIHelper.ShowError("Lựa chọn không hợp lệ!");
            }
        }

        private async Task<DateTime?> SelectBookingDate()
        {
            UIHelper.DrawHeader("CHỌN NGÀY ĐẶT LỊCH");

            while (true)
            {
                string dateInput = UIHelper.GetInput("Nhập ngày đặt lịch (dd/MM/yyyy)");
                
                if (dateInput == "0")
                    return null;

                if (DateTime.TryParseExact(dateInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                {
                    if (date.Date < DateTime.Today)
                    {
                        UIHelper.ShowError("Không thể đặt lịch cho ngày trong quá khứ!");
                        continue;
                    }

                    if (date.Date > DateTime.Today.AddMonths(2))
                    {
                        UIHelper.ShowError("Chỉ có thể đặt lịch tối đa 2 tháng trước!");
                        continue;
                    }

                    return date;
                }
                
                UIHelper.ShowError("Định dạng ngày không hợp lệ! Vui lòng nhập theo định dạng dd/MM/yyyy");
            }
        }

        private async Task<TimeSlot?> SelectTimeSlot(DateTime date, Service service)
        {
            UIHelper.DrawHeader($"CHỌN KHUNG GIỜ - {date:dd/MM/yyyy}");

            try
            {
                var timeSlots = await _bookingService.GenerateTimeSlots(date, service.DurationMinutes);

                if (!timeSlots.Any())
                {
                    UIHelper.ShowWarning("Không có khung giờ trống cho ngày này!");
                    return null;
                }

                var slotList = timeSlots.ToList();
                var headers = new[] { "STT", "Buổi", "Khung giờ" };
                var rows = slotList.Select((s, i) => new[]
                {
                    (i + 1).ToString(),
                    s.Session,
                    s.DisplayText
                }).ToArray();

                UIHelper.DrawTable(headers, rows);

                while (true)
                {
                    string choiceInput = UIHelper.GetInput("Chọn khung giờ (nhập số thứ tự)");
                    
                    if (choiceInput == "0")
                        return null;

                    if (int.TryParse(choiceInput, out int choice) && choice >= 1 && choice <= slotList.Count)
                    {
                        return slotList[choice - 1];
                    }

                    UIHelper.ShowError("Lựa chọn không hợp lệ!");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tạo khung giờ: {ex.Message}");
                return null;
            }
        }

        private async Task<Barber?> SelectBarber(IEnumerable<Barber> barbers)
        {
            UIHelper.DrawHeader("CHỌN BARBER");

            var barberList = barbers.ToList();
            
            if (!barberList.Any())
            {
                UIHelper.ShowWarning("Không có barber nào khả dụng trong khung giờ này!");
                return null;
            }

            var headers = new[] { "STT", "Tên Barber", "Số điện thoại" };
            var rows = barberList.Select((b, i) => new[]
            {
                (i + 1).ToString(),
                b.FullName,
                b.Phone
            }).ToArray();

            UIHelper.DrawTable(headers, rows);

            while (true)
            {
                string choiceInput = UIHelper.GetInput("Chọn Barber (nhập số thứ tự)");
                
                if (choiceInput == "0")
                    return null;

                if (int.TryParse(choiceInput, out int choice) && choice >= 1 && choice <= barberList.Count)
                {
                    return barberList[choice - 1];
                }

                UIHelper.ShowError("Lựa chọn không hợp lệ!");
            }
        }

        private async Task<bool> ConfirmBooking(Barber barber, Service service, DateTime startTime, DateTime endTime)
        {
            UIHelper.DrawHeader("XÁC NHẬN ĐẶT LỊCH");

            Console.WriteLine($"Khách hàng: {_currentCustomer!.FullName}");
            Console.WriteLine($"Barber: {barber.FullName}");
            Console.WriteLine($"Dịch vụ: {service.Name}");
            Console.WriteLine($"Ngày: {startTime:dd/MM/yyyy}");
            Console.WriteLine($"Giờ: {startTime:HH:mm} - {endTime:HH:mm}");
            Console.WriteLine($"Thời gian: {service.DurationMinutes} phút");
            Console.WriteLine($"Giá: {service.Price:N0} VND");

            string confirm = UIHelper.GetInput("Xác nhận đặt lịch này? (Y/N)");
            if (confirm.ToUpper() == "Y")
            {
                try
                {
                    int bookingId = await _bookingService.CreateAsync(
                        _currentCustomer.CustomerId,
                        barber.BarberId,
                        service.ServiceId,
                        startTime,
                        endTime
                    );

                    if (bookingId > 0)
                    {
                        UIHelper.ShowSuccess($"Đặt lịch thành công! Mã booking: {bookingId}");
                        return true;
                    }
                    else
                    {
                        UIHelper.ShowError("Đặt lịch thất bại!");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    UIHelper.ShowError($"Lỗi khi đặt lịch: {ex.Message}");
                    return false;
                }
            }
            return false;
        }

        private async Task ViewMyBookingsAsync()
        {
            UIHelper.DrawHeader("LỊCH HẸN CỦA TÔI");

            try
            {
                var bookings = (await _bookingService.GetByCustomerIdAsync(_currentCustomer!.CustomerId))
                    .Where(b => b.Status != "Cancelled")
                    .OrderBy(b => b.StartTime);

                if (!bookings.Any())
                {
                    UIHelper.ShowInfo("Không có lịch hẹn nào!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var headers = new[] { "ID", "Ngày/Giờ", "Dịch vụ", "Giá", "Trạng thái" };
                var rows = new List<string[]>();

                foreach (var booking in bookings)
                {
                    var service = await _serviceService.GetByIdAsync(booking.ServiceId);
                    rows.Add(new[]
                    {
                        booking.BookingId.ToString(),
                        booking.StartTime.ToString("dd/MM/yyyy HH:mm"),
                        service?.Name ?? "Không xác định",
                        service != null ? $"{service.Price:N0}" : "0",
                        booking.Status
                    });
                }

                UIHelper.DrawTable(headers, rows.ToArray());
                UIHelper.ShowInfo($"Tổng số lịch hẹn: {bookings.Count()}");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải lịch hẹn: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task CancelBookingAsync()
        {
            UIHelper.DrawHeader("HỦY LỊCH HẸN");

            try
            {
                var bookings = (await _bookingService.GetByCustomerIdAsync(_currentCustomer!.CustomerId))
                    .Where(b => b.Status == "Booked" && b.StartTime > DateTime.Now.AddHours(2))
                    .OrderBy(b => b.StartTime);

                if (!bookings.Any())
                {
                    UIHelper.ShowInfo("Không có lịch hẹn nào có thể hủy! (Chỉ có thể hủy lịch hẹn trước 2 giờ)");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var headers = new[] { "ID", "Ngày/Giờ", "Dịch vụ", "Barber" };
                var rows = new List<string[]>();

                foreach (var booking in bookings)
                {
                    var service = await _serviceService.GetByIdAsync(booking.ServiceId);
                    var barber = await _barberService.GetByIdAsync(booking.BarberId);
                    
                    rows.Add(new[]
                    {
                        booking.BookingId.ToString(),
                        booking.StartTime.ToString("dd/MM/yyyy HH:mm"),
                        service?.Name ?? "Không xác định",
                        barber?.FullName ?? "Không xác định"
                    });
                }

                UIHelper.DrawTable(headers, rows.ToArray());

                string bookingIdInput = UIHelper.GetInput("Nhập ID lịch hẹn cần hủy (0 để quay lại)");
                
                if (bookingIdInput == "0")
                    return;

                if (int.TryParse(bookingIdInput, out int bookingId))
                {
                    var bookingToCancel = bookings.FirstOrDefault(b => b.BookingId == bookingId);
                    if (bookingToCancel != null)
                    {
                        var service = await _serviceService.GetByIdAsync(bookingToCancel.ServiceId);
                        
                        UIHelper.ShowWarning($"Bạn sẽ hủy lịch hẹn: {service?.Name} vào {bookingToCancel.StartTime:dd/MM/yyyy HH:mm}");
                        string confirm = UIHelper.GetInput("Xác nhận hủy? (Y/N)");
                        
                        if (confirm.ToUpper() == "Y")
                        {
                            bool success = await _bookingService.CancelAsync(bookingId);

                            if (success)
                            {
                                UIHelper.ShowSuccess("Hủy lịch hẹn thành công!");
                            }
                            else
                            {
                                UIHelper.ShowError("Hủy lịch hẹn thất bại!");
                            }
                        }
                    }
                    else
                    {
                        UIHelper.ShowError("ID lịch hẹn không hợp lệ hoặc không thể hủy!");
                    }
                }
                else
                {
                    UIHelper.ShowError("ID lịch hẹn không hợp lệ!");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi hủy lịch hẹn: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task ViewBookingHistoryAsync()
        {
            UIHelper.DrawHeader("LỊCH SỬ BOOKING");

            try
            {
                var bookings = (await _bookingService.GetByCustomerIdAsync(_currentCustomer!.CustomerId))
                    .OrderByDescending(b => b.StartTime);

                if (!bookings.Any())
                {
                    UIHelper.ShowInfo("Không có lịch sử booking nào!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                var headers = new[] { "ID", "Ngày/Giờ", "Dịch vụ", "Barber", "Trạng thái", "Giá" };
                var rows = new List<string[]>();

                decimal totalSpent = 0;
                int completedCount = 0;

                foreach (var booking in bookings)
                {
                    var service = await _serviceService.GetByIdAsync(booking.ServiceId);
                    var barber = await _barberService.GetByIdAsync(booking.BarberId);
                    
                    if (booking.Status == "Completed" && service != null)
                    {
                        totalSpent += service.Price;
                        completedCount++;
                    }
                    
                    rows.Add(new[]
                    {
                        booking.BookingId.ToString(),
                        booking.StartTime.ToString("dd/MM/yyyy HH:mm"),
                        service?.Name ?? "Không xác định",
                        barber?.FullName ?? "Không xác định",
                        booking.Status,
                        service != null ? $"{service.Price:N0}" : "0"
                    });
                }

                UIHelper.DrawTable(headers, rows.ToArray());
                
                Console.WriteLine("\n=== THỐNG KÊ ===");
                Console.WriteLine($"Tổng số lần sử dụng dịch vụ: {completedCount}");
                Console.WriteLine($"Tổng số tiền đã chi: {totalSpent:N0} VND");
                Console.WriteLine($"Tổng số booking: {bookings.Count()}");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Lỗi khi tải lịch sử: {ex.Message}");
            }

            UIHelper.WaitForAnyKey();
        }

        private async Task UpdateProfileAsync()
        {
            UIHelper.DrawHeader("CẬP NHẬT THÔNG TIN CÁ NHÂN");

            try
            {
                Console.WriteLine("Thông tin hiện tại:");
                Console.WriteLine($"Tên đăng nhập: {_currentCustomer!.Username}");
                Console.WriteLine($"Họ tên: {_currentCustomer.FullName}");
                Console.WriteLine($"Email: {_currentCustomer.Email}");
                Console.WriteLine($"Số điện thoại: {_currentCustomer.Phone}");

                Console.WriteLine("\nNhập thông tin mới (Enter để giữ nguyên):");

                string newUsername = UIHelper.GetInput("Tên đăng nhập mới");
                string newFullName = UIHelper.GetInput("Họ tên mới");
                string newEmail = UIHelper.GetInput("Email mới");
                string newPhone = UIHelper.GetInput("Số điện thoại mới");

                // Validate input
                if (!string.IsNullOrEmpty(newPhone) && !ValidationHelper.IsValidPhone(newPhone))
                {
                    UIHelper.ShowError("Số điện thoại không hợp lệ!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                if (!string.IsNullOrEmpty(newEmail) && !ValidationHelper.IsValidEmail(newEmail))
                {
                    UIHelper.ShowError("Email không hợp lệ!");
                    UIHelper.WaitForAnyKey();
                    return;
                }

                bool success = await _customerService.UpdateAsync(
                    _currentCustomer.CustomerId,
                    string.IsNullOrEmpty(newUsername) ? _currentCustomer.Username : newUsername,
                    string.IsNullOrEmpty(newFullName) ? _currentCustomer.FullName : newFullName,
                    string.IsNullOrEmpty(newPhone) ? _currentCustomer.Phone : newPhone,
                    string.IsNullOrEmpty(newEmail) ? _currentCustomer.Email : newEmail
                );

                if (success)
                {
                    _currentCustomer = await _customerService.GetByIdAsync(_currentCustomer.CustomerId);
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
    }
}
