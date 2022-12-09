﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using tp_final.Commands;
using tp_final.Stores;
using tp_final.Views;

namespace tp_final.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private readonly NavigationStore navigationStore;
        private string username;
        private string password;
        public DelegateCommand GoToRegisterCommand { get; set; }
        public DelegateCommand LoginCommand { get; set; }
        public LoginViewModel(NavigationStore _navigationStore) {
            navigationStore = _navigationStore;
            GoToRegisterCommand = new DelegateCommand(GoToRegister);
            LoginCommand = new DelegateCommand(Login);
            
        }

        public string Username { 
            get => username; 
            set => username = value;
        }

        public string Password
        {
            get => password;
            set => password = value;
        }

        public void GoToRegister()
        {
            navigationStore.CurrentViewModel = new RegisterViewModel(navigationStore);
        }

        public void Login()
        {

            string username = Username;
            string password = Password;
            MessageBox.Show(Username + Password);
            navigationStore.CurrentViewModel = new MainPlayerViewModel(navigationStore);
        }
    }
}
