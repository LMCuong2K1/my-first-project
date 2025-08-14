using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace BarberShopManagement.Presentation.Utils
{
    public static class ValidationHelper
    {
        public static bool IsValidPhone(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone) &&
                   phone.Length == 10 &&
                   phone.StartsWith("0") &&
                   phone.All(char.IsDigit);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var emailRegex = new Regex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        public static string GetValidPhone(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var phone = Console.ReadLine()?.Trim();

                if (IsValidPhone(phone))
                    return phone!;

                Console.WriteLine("Số điện thoại không hợp lệ! Vui lòng nhập 10 số bắt đầu bằng 0.");
            }
        }

        public static string GetValidEmail(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var email = Console.ReadLine()?.Trim();

                if (IsValidEmail(email))
                    return email!;

                Console.WriteLine("Email không hợp lệ! Vui lòng nhập đúng định dạng email.");
            }
        }

        public static int GetValidInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine()?.Trim();

                if (int.TryParse(input, out int result) && result >= min && result <= max)
                    return result;

                Console.WriteLine($"Vui lòng nhập số nguyên hợp lệ từ {min} đến {max}.");
            }
        }

        public static decimal GetValidDecimal(string prompt, decimal min = 0)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine()?.Trim();

                if (decimal.TryParse(input, out decimal result) && result >= min)
                    return result;

                Console.WriteLine($"Vui lòng nhập số thập phân hợp lệ >= {min}.");
            }
        }

        // ✅ MỚI: Validate thời gian booking
        public static bool IsValidBookingTime(DateTime startTime, DateTime endTime)
        {
            // Kiểm tra trong khung giờ làm việc: 8-12h và 14-18h
            var startHour = startTime.Hour;
            var endHour = endTime.Hour;
            var startMinute = startTime.Minute;
            var endMinute = endTime.Minute;

            // Khung sáng: 8:00 - 12:00
            bool isValidMorning = (startHour >= 8 && startHour < 12) &&
                                  (endHour <= 12 || (endHour == 12 && endMinute == 0));

            // Khung chiều: 14:00 - 18:00
            bool isValidAfternoon = (startHour >= 14 && startHour < 18) &&
                                    (endHour <= 18 || (endHour == 18 && endMinute == 0));

            // Không được span qua 2 session
            bool notSpanSession = !((startHour < 12 && endHour >= 14) || (startHour < 14 && endHour >= 14));

            return (isValidMorning || isValidAfternoon) && notSpanSession;
        }

        // ✅ MỚI: Parse danh sách dịch vụ
        public static int[] ParseServiceIds(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new int[0];

            try
            {
                return input.Split(',')
                           .Select(s => s.Trim())
                           .Where(s => !string.IsNullOrEmpty(s))
                           .Select(int.Parse)
                           .Distinct()
                           .ToArray();
            }
            catch
            {
                return new int[0];
            }
        }

        // ✅ MỚI: Validate trong 30 ngày
        public static bool IsWithin30Days(DateTime date)
        {
            var today = DateTime.Today;
            var maxDate = today.AddDays(30);
            return date.Date >= today && date.Date <= maxDate;
        }
        
    }
}
