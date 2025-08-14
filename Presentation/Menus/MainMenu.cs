using System;
using BarberShopManagement.Presentation.Utils;

namespace BarberShopManagement.Presentation.Menus
{
    public class MainMenu
    {
        private readonly AdminMenu _adminMenu;
        private readonly BarberMenu _barberMenu;
        private readonly CustomerMenu _customerMenu;

        public MainMenu(
            AdminMenu adminMenu,
            BarberMenu barberMenu,
            CustomerMenu customerMenu)
        {
            _adminMenu = adminMenu;
            _barberMenu = barberMenu;
            _customerMenu = customerMenu;
        }

        public void Show()
        {
            while (true)
            {
                UIHelper.DrawHeader("HỆ THỐNG QUẢN LÝ BARBER SHOP");
                
                string[] menuItems = {
                    "Đăng nhập Admin",
                    "Đăng nhập Barber",
                    "Đăng nhập Khách hàng",
                    "Đăng ký Khách hàng"
                };
                
                UIHelper.DrawMenu(menuItems, "MENU CHÍNH");

                string choice = UIHelper.GetInput("Lựa chọn của bạn");
                
                switch (choice)
                {
                    case "1":
                        _adminMenu.ShowLoginAsync().Wait();
                        break;
                    case "2":
                        _barberMenu.ShowLoginAsync().Wait();
                        break;
                    case "3":
                        _customerMenu.ShowLoginAsync().Wait();
                        break;
                    case "4":
                        _customerMenu.ShowRegisterAsync().Wait();
                        break;
                    case "0":
                        UIHelper.ShowInfo("Cảm ơn bạn đã sử dụng hệ thống!");
                        UIHelper.WaitForAnyKey();
                        return;
                    default:
                        UIHelper.ShowError("Lựa chọn không hợp lệ!");
                        UIHelper.WaitForAnyKey();
                        break;
                }
            }
        }
    }
}
