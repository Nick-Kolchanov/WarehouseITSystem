using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace WarehouseITSystem.ViewModels
{
    internal class LoginViewModel : Screen
    {
        private readonly WindowManager windowManager;

        public LoginViewModel(): base()
        {
            windowManager = new WindowManager();
        }

        private string username = "";
        public string Username
        {
            get => username;
            set
            {
                username = value;
                NotifyOfPropertyChange(() => Username);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        private string password = "";
        public string Password
        {
            get => password;
            set
            {
                password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public bool CanLogin
        {
            get { return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password); }
        }

        public void Login()
        {
            var passHash = Utils.PasswordHash.GetHash(Password.ToCharArray());
            using WarehouseContext db = new();
            if (!db.Users.Where(u => u.IsAdmin == true).Any())
            {
                db.Users.Add(new Models.User { Name = "admin", PasswordHash = Utils.PasswordHash.GetHash("admin".ToCharArray()), IsAdmin = true });
                db.SaveChanges();
            }

            var user = db.Users.Where(usr => usr.Name == Username && usr.PasswordHash == passHash).FirstOrDefault();
            if (user == null)
            {
                MessageBox.Show("Введены неверные данные. Если вы забыли логин или пароль, обратитесь за помощью к системному администратору.");
                return;
            }
            else
            {
                if (user.IsAdmin)
                {
                    windowManager.ShowWindowAsync(new ShellViewModel());
                    TryCloseAsync();
                }
                else
                {
                    windowManager.ShowWindowAsync(new UserShellViewModel());
                    TryCloseAsync();
                }
            }
        }
    }
}
