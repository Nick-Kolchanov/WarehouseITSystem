using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using WarehouseITSystem.Models;

namespace WarehouseITSystem.ViewModels
{
    internal class AddUserViewModel : Screen
    {
        public AddUserViewModel() : base() { }

        public AddUserViewModel(UserToScreen user) : base()
        {
            oldUser = user;
            Username = user.Name;
            IsAdmin = user.IsAdmin;
        }

        private readonly UserToScreen? oldUser;

        private string username = "";
        public string Username
        {
            get => username;
            set
            {
                username = value;
                NotifyOfPropertyChange(() => Username);
                NotifyOfPropertyChange(() => CanRegisterUser);
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
                NotifyOfPropertyChange(() => CanRegisterUser);
            }
        }

        private bool isAdmin;
        public bool IsAdmin 
        { 
            get => isAdmin; 
            set 
            { 
                isAdmin = value; 
                NotifyOfPropertyChange(() => IsAdmin); 
            } 
        }

        public bool CanRegisterUser
        {
            get { return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password); }
        }

        public async void RegisterUser()
        {
            if (!Validate())
                return;

            User user = new()
            {
                Name = Username,
                PasswordHash = Utils.PasswordHash.GetHash(Password.ToCharArray()),
                IsAdmin = IsAdmin
            };

            using var db = new WarehouseContext();
            if (oldUser != null)
            {
                var foundUser = db.Users.Where(u => u.Id == oldUser.Id).FirstOrDefault();
                if (foundUser != null)
                {
                    foundUser.Name = user.Name;
                    foundUser.PasswordHash = user.PasswordHash;
                    foundUser.IsAdmin = user.IsAdmin;
                }
            }
            else
            {
                db.Users.Add(user);
            }
            
            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        private bool Validate()
        {
            if (Username.Length < 4)
            {
                MessageBox.Show("Длина имени пользователя не может быть меньше 4");
                return false;
            }

            if (Password.Length < 6)
            {
                MessageBox.Show("Длина пароля не может быть меньше 6");
                return false;
            }

            if (Password.All(char.IsLower))
            {
                MessageBox.Show("Пароль должен содержать заглавные буквы");
                return false;
            }    

            using WarehouseContext db = new();
            var user = db.Users.Where(usr => usr.Name == Username && usr.PasswordHash == Utils.PasswordHash.GetHash(Password.ToCharArray())).FirstOrDefault();
            if (user != null)
            {
                // changing only IsAdmin
                if (oldUser!= null && IsAdmin != oldUser.IsAdmin)
                {
                    return true;
                }

                MessageBox.Show("Такой пользователь уже есть");
                return false;
            }

            return true;
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }
    }
}
