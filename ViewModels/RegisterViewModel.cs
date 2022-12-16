﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using tp_final.Commands;
using tp_final.Models;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    internal class RegisterViewModel : BaseViewModel
    {
        private string username;
        private string password;
        private string email;
        private readonly NavigationStore navigationStore;
        public DelegateCommand BackToLoginCommand { get; set; }
        public DelegateCommand RegisterCommand { get; set; }

        public RegisterViewModel(NavigationStore _navigationStore) {
            navigationStore = _navigationStore;
            BackToLoginCommand = new DelegateCommand(BackToLogin);
            RegisterCommand = new DelegateCommand(Register);
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
            get => password;
            set => password = value;
        }

        public void BackToLogin()
        {
            navigationStore.CurrentViewModel = new LoginViewModel(navigationStore);
        }

        public async void Register()
        {
            User user = await User.AddUserAsync(Username, Password, Email);

            Application.Current.Properties["CurrentUserAdmin"] = user.isAdmin;
            Application.Current.Properties["CurrentUserId"] = user.id;
            navigationStore.CurrentViewModel = new MainPlayerViewModel(navigationStore);
        }
    }
}
