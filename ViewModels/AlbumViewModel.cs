﻿using System;
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
    public class AlbumViewModel : BaseViewModel
    {
        //CollectionView
        ICollectionView albumlistViewSource;
        //Delegate Command
        public DelegateCommand GoToAdminCommand { get; set; }
        public DelegateCommand GoToMainPlayerCommand { get; set; }
        public DelegateCommand GoToLogoutCommand { get; set; }
        public DelegateCommand AddAlbumCommand { get; private set; }
        public DelegateCommand DeleteAlbumCommand { get; private set; }
        public DelegateCommand ShuffleCommand { get; private set; }
        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand PreviousCommand { get; private set; }
        public DelegateCommand LikeCommand { get; private set; }
        //Navigation
        private readonly NavigationStore navigationStore;
       
        public AlbumViewModel(NavigationStore _navigationStore){

            //var temp = Application.Current.Properties["CurrentUserId"];
            //MessageBox.Show(temp.ToString());

            //CollectionView
            SetAlbumListAsync();
            
            //Add
            AddAlbumCommand = new DelegateCommand(AddAlbum);

            //Delete
            DeleteAlbumCommand = new DelegateCommand(DeleteAlbum);

            //CommandMusic
            ShuffleCommand = new DelegateCommand(Shuffle);
            PlayCommand = new DelegateCommand(Play);
            PauseCommand = new DelegateCommand(Pause);
            NextCommand = new DelegateCommand(Next);
            PreviousCommand = new DelegateCommand(Previous);
            LikeCommand = new DelegateCommand(Like);

            //Navigation between page
            navigationStore = _navigationStore;
            GoToAdminCommand = new DelegateCommand(GoToAdmin);
            GoToMainPlayerCommand = new DelegateCommand(GoToMainPlayer);
            GoToLogoutCommand = new DelegateCommand(GoToLogout);
        }

        public void AddAlbum()
        {
            // Playlist _newAlbum = new Playlist() { title = "Test" };
            // testDataServices.LesPlaylists.Add(_newAlbum);
        }
        public void DeleteAlbum()
        {
            if (AlbumlistViewSource.CurrentItem != null)
            {
                MessageBox.Show($"{AlbumlistViewSource.CurrentItem}");
                MessageBox.Show($"{ Application.Current.Properties["CurrentUserId"]}");
                //TODO: Changer la comparaison pour qu'il soit fonctionnel
                if(Application.Current.Properties["user_id"] == Application.Current.Properties["CurrentUserId"])
                {
                    string messaBoxText = "Êtes-vous certain de vouloir supprimer cet album?";
                    string caption = "Vous êtes sur le point de supprimer un album";
                    MessageBoxButton button = MessageBoxButton.OKCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult result = MessageBox.Show(messaBoxText, caption, button, icon);
                    if (result == MessageBoxResult.OK)
                    {
                        messaBoxText = "Voulez-vous supprimer les morceaux?";
                        caption = "Vous êtes sur le point de supprimer les morceaux de l'album";
                        button = MessageBoxButton.OKCancel;
                        icon = MessageBoxImage.Warning;
                        result = MessageBox.Show(messaBoxText, caption, button, icon);
                        if (result == MessageBoxResult.OK)
                        {
                            MessageBox.Show("supprimer");
                            //foreach (morceau in album)
                            //{
                            //    if (morceau != currentUser)
                            //    {
                            //        MessageBox.Show("move to unknown");
                            //    }
                            //    else
                            //    {
                            //        MessageBox.Show("Delete");
                            //    }
                            //}

                        }
                        else
                        {
                            MessageBox.Show("move to unknown");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez supprimer un album qui vous appartient");
                }
            }
            else
            {
                MessageBox.Show("Vous devez sélectionner un album");
            }
        }
        public void Shuffle() { }
        public void Play() { }
        public void Pause() { }
        public void Next() { }
        public void Previous() { }
        public void Like() { }
        public void GoToAdmin()
        {
            
            if(Application.Current.Properties["CurrentUserAdmin"].ToString() == "1")
            {
                navigationStore.CurrentViewModel = new AdminUserViewModel(navigationStore);
            } else {
                MessageBox.Show("you are not an admin!");
            }
        }
        public void GoToMainPlayer()
        {
            navigationStore.CurrentViewModel = new MainPlayerViewModel(navigationStore);
        }
        public void GoToLogout()
        {
            navigationStore.CurrentViewModel = new LoginViewModel(navigationStore);
        }
        public ICollectionView AlbumlistViewSource
        {
            get => albumlistViewSource;
            set
            {
                albumlistViewSource = value;
                OnPropertyChanged();
            }
        }
        private async void SetAlbumListAsync()
        {
            var albums = await Playlist.GetAllAlbumsAsync();
            AlbumlistViewSource = CollectionViewSource.GetDefaultView(albums);
        }
    }
}
