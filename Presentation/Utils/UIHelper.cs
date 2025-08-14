using System;
using System.Collections.Generic;
using System.Linq;

namespace BarberShopManagement.Presentation.Utils
{
    public static class UIHelper
    {
        private const int ScreenWidth = 80;

        public static void DrawHeader(string title)
        {
            Console.Clear();
            Console.WriteLine();
            CenterText(title.ToUpper(), '#');
            Console.WriteLine();
        }

        public static void DrawMenu(string[] options, string currentMenu)
        {
            Console.WriteLine();
            CenterText($"[ {currentMenu.ToUpper()} ]", '=');
            Console.WriteLine();

            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {options[i]}");
            }
            Console.WriteLine($"  0. Quay lại/Thoát");
            Console.WriteLine();
            CenterText("", '=');
            Console.WriteLine();
        }

        public static void DrawTable(string[] headers, string[][] rows)
        {
            var columnWidths = CalculateColumnWidths(headers, rows);

            PrintSeparator(columnWidths, '=');
            PrintRow(headers, columnWidths);
            PrintSeparator(columnWidths, '-');

            foreach (var row in rows)
            {
                PrintRow(row, columnWidths);
            }

            PrintSeparator(columnWidths, '=');
            Console.WriteLine();
        }

        public static void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" [+] {message}");
            Console.ResetColor();
        }

        public static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" [!] {message}");
            Console.ResetColor();
        }

        public static void ShowWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" [*] {message}");
            Console.ResetColor();
        }

        public static void ShowInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" [i] {message}");
            Console.ResetColor();
        }

        public static string GetInput(string prompt)
        {
            Console.Write($" {prompt}: ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        public static void WaitForAnyKey()
        {
            Console.WriteLine("\n Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }

        private static int[] CalculateColumnWidths(string[] headers, string[][] rows)
        {
            int[] widths = new int[headers.Length];

            for (int i = 0; i < headers.Length; i++)
            {
                int maxWidth = headers[i].Length;
                foreach (var row in rows)
                {
                    if (i < row.Length && row[i]?.Length > maxWidth)
                    {
                        maxWidth = row[i].Length;
                    }
                }
                widths[i] = maxWidth + 2;
            }

            return widths;
        }

        private static void PrintSeparator(int[] widths, char separatorChar)
        {
            Console.Write(" ");
            foreach (int width in widths)
            {
                Console.Write(new string(separatorChar, width + 2));
            }
            Console.WriteLine();
        }

        private static void PrintRow(string[] cells, int[] widths)
        {
            Console.Write(" ");
            for (int i = 0; i < cells.Length; i++)
            {
                string cell = cells[i]?.PadRight(widths[i]) ?? new string(' ', widths[i]);
                Console.Write($"| {cell} ");
            }
            Console.WriteLine("|");
        }

        private static void CenterText(string text, char padChar = ' ')
        {
            if (string.IsNullOrEmpty(text))
            {
                Console.WriteLine(new string(padChar, ScreenWidth));
                return;
            }

            int totalPadding = ScreenWidth - text.Length - 2;
            int leftPadding = totalPadding / 2;
            int rightPadding = totalPadding - leftPadding;

            Console.WriteLine(
                new string(padChar, leftPadding) +
                $" {text} " +
                new string(padChar, rightPadding)
            );
        }
        public static void ShowBookingError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n⏰ LỖI ĐẶT LỊCH: {message}");
        Console.ResetColor();
    }
    }
}
