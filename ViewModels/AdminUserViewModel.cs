﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public DelegateCommand GoToAlbumCommand { get; set; }
        public DelegateCommand GoToMainCommand { get; set; }
        public AdminUserViewModel(NavigationStore _navigationStore) {
            SetUserListAsync();
            navigationStore = _navigationStore;
            GoToAlbumCommand = new DelegateCommand(GoToAlbum);
            GoToMainCommand = new DelegateCommand(GoToMain);

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

        public void GoToAlbum()
        {
            navigationStore.CurrentViewModel = new AlbumViewModel(navigationStore);
        }

        public void GoToMain()
        {
            navigationStore.CurrentViewModel = new MainPlayerViewModel(navigationStore);
        }

        private async void SetUserListAsync() {
            var users = await User.getAllUsersAsync();
            UserViewSource = CollectionViewSource.GetDefaultView(users);
        }
    }


    
}
