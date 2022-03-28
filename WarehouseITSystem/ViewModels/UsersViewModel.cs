using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using WarehouseITSystem.Models;

namespace WarehouseITSystem.ViewModels
{
    internal class UsersViewModel : Screen
    {
        readonly IWindowManager manager = new WindowManager();
        public UsersViewModel() : base()
        {
            LoadValues();
        }

        public void RefreshValues()
        {
            LoadValues();
        }
        private void LoadValues()
        {
            WarehouseContext db = new();
            db.Users.Load();
            Users = new BindableCollection<UserToScreen>(db.Users.Select(u => new UserToScreen { Id = u.Id, Name = u.Name, IsAdmin = u.IsAdmin }));
        }

        private IObservableCollection<UserToScreen> users = null!;
        public IObservableCollection<UserToScreen> Users
        {
            get => users;
            set
            {
                users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }

        private UserToScreen? selectedUser;
        public UserToScreen? SelectedUser
        {
            get => selectedUser;
            set
            {
                selectedUser = value;
                NotifyOfPropertyChange(() => SelectedUser);
                NotifyOfPropertyChange(() => CanDeleteUser);
                NotifyOfPropertyChange(() => CanChangeUser);
            }
        }

        private string filterText = string.Empty;
        public string FilterText
        {
            get => filterText;
            set
            {
                filterText = value;
                NotifyOfPropertyChange(() => FilterText);
                NotifyOfPropertyChange(() => CanSearchFilter);
            }
        }

        public bool CanSearchFilter
        {
            get => !string.IsNullOrWhiteSpace(FilterText);
        }

        public void SearchFilter()
        {
            if (int.TryParse(FilterText, out int searchId))
            {
                foreach (var user in Users)
                {
                    if (user.Id == searchId)
                    {
                        SelectedUser = user;
                        return;
                    }
                }
            }
            else
            {
                foreach (var user in Users)
                {
                    if (user.Name.Contains(FilterText))
                    {
                        SelectedUser = user;
                        return;
                    }
                }
            }

            MessageBox.Show("Пользователь не найден");
        }

        public async void AddUser()
        {
            var response = await manager.ShowDialogAsync(new AddUserViewModel());
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanChangeUser
        {
            get => SelectedUser != null;
        }
        public async void ChangeUser()
        {
            var response = await manager.ShowDialogAsync(new AddUserViewModel(SelectedUser!));
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanDeleteUser
        {
            get => SelectedUser != null;
        }
        public async void DeleteUser()
        {
            using WarehouseContext db = new();

            var foundUser = db.Users.Where(x => x.Id == SelectedUser!.Id).FirstOrDefault();

            if (foundUser != null)
            {
                Users.Remove(SelectedUser!);
                db.Users.Remove(foundUser);
                await db.SaveChangesAsync();
            }
        } 
    }
}
