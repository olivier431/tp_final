using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using tp_final.Commands;
using tp_final.Models;
using tp_final.Services;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    internal class AdminUserViewModel : BaseViewModel
    {
        ICollectionView userViewSource;
        private readonly NavigationStore navigationStore;
        private string username;
        private string password;
        private string email;
        public DelegateCommand GoToAlbumCommand { get; set; }
        public DelegateCommand GoToMainCommand { get; set; }
        public DelegateCommand UpdateUserCommand { get; set; }
        public DelegateCommand DeleteUserCommand { get; set; }
        public DelegateCommand AddUserCommand { get; set; }

        public AdminUserViewModel(NavigationStore _navigationStore) {
            SetUserListAsync();
            navigationStore = _navigationStore;
            GoToAlbumCommand = new DelegateCommand(GoToAlbum);
            GoToMainCommand = new DelegateCommand(GoToMain);
            UpdateUserCommand = new DelegateCommand(UpdateUser);
            DeleteUserCommand = new DelegateCommand(DeleteUser);
            AddUserCommand = new DelegateCommand(AddUser);

        }

        public ICollectionView UserViewSource
        {
            get => userViewSource;
            set
            {
                userViewSource = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => username;
            set => username = value;
        }

        public string Password
        {
            get => password;
            set => password = value;
        }

        public string Email
        {
            get => email;
            set => email = value;
        }
        public void GoToAlbum()
        {
            navigationStore.CurrentViewModel = new AlbumViewModel(navigationStore);
        }

        public void GoToMain()
        {
            navigationStore.CurrentViewModel = new MainPlayerViewModel(navigationStore);
        }

        public async void UpdateUser()
        {
            User user = (User)userViewSource.CurrentItem;
            await User.UpdateUserAsync(user.username, user.email, user.pwd, user.id);
            SetUserListAsync();

        }

        public async void DeleteUser()
        {
            User user = (User)userViewSource.CurrentItem;
            if (Application.Current.Properties["CurrentUserId"].ToString() != user.id.ToString())
            {
                await User.DeleteUserAsync(user.id);
            }
            else
            {
                MessageBox.Show("you can't delete the login user");
            }
            SetUserListAsync();

        }

        public async void AddUser()
        {
            if ((Username != null) && (Password != null) && (Email != null))
            {
                await User.AddUserAsync(Username, Password, Email);

                SetUserListAsync();
            }
            else
            {
                MessageBox.Show("a box is empty !");
            }
            
        }

        private async void SetUserListAsync() {
            var users = await User.GetAllUsersAsync();
            UserViewSource = CollectionViewSource.GetDefaultView(users);
        }
    }


    
}
