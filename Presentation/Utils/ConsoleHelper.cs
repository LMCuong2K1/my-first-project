using System;
using System.Text;

namespace BarberShopManagement.Presentation.Utils
{
    public static class ConsoleHelper
    {
        public static string GetValidPassword(string prompt)
        {
            Console.Write(prompt);
            var password = new StringBuilder();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password.ToString();
        }

        public static bool GetYesNo(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (Y/N): ");
                var input = Console.ReadLine()?.Trim().ToUpper();
                
                if (input == "Y" || input == "YES")
                    return true;
                if (input == "N" || input == "NO")
                    return false;
                
                Console.WriteLine("Vui lòng nhập Y hoặc N.");
            }
        }
    }
}
