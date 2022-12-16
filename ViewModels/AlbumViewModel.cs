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
        ICollectionView tuneViewSource;
        ICollectionView albumlistViewSource;

        public DelegateCommand GoToAdminCommand { get; set; }
        public DelegateCommand GoToMainPlayerCommand { get; set; }
        public DelegateCommand GoToLogoutCommand { get; set; }

        public DelegateCommand AddAlbumCommand { get; private set; }
        public DelegateCommand DeleteAlbumCommand { get; private set; }
        public DelegateCommand AddTuneCommand { get; private set; }
        public DelegateCommand DeleteTuneCommand { get; private set; }
        public DelegateCommand ShuffleCommand { get; private set; }
        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand PreviousCommand { get; private set; }
        public DelegateCommand LikeCommand { get; private set; }


      //  TestDataServices testDataServices = new TestDataServices();
        private readonly NavigationStore navigationStore;

        public AlbumViewModel(NavigationStore _navigationStore){

            //var temp = Application.Current.Properties["CurrentUser"];
            //MessageBox.Show(temp.ToString());

            //CollectionView
            SetAlbumListAsync();
            SetTuneListAsync();
            
            //Add
            AddAlbumCommand = new DelegateCommand(AddAlbum);
            AddTuneCommand = new DelegateCommand(AddTune);

            //Delete
            DeleteAlbumCommand = new DelegateCommand(DeleteAlbum);
            DeleteTuneCommand = new DelegateCommand(DeleteTune);

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
            //TODO: Prendre le bon album pour delete
            if (AlbumlistViewSource.CurrentItem != null)
            {
                string messaBoxText = "Êtes-vous certain de vouloir supprimer cet album?";
                string caption = "Vous êtes sur le point de détruire un album";
                MessageBoxButton button = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messaBoxText, caption, button, icon);
                if (result == MessageBoxResult.OK)
                {
                  //  Playlists album = (Playlists)PlaylistViewSource.CurrentItem;
                  //  testDataServices.LesPlaylists.Remove(album);
                }
            }
            else
            {
                MessageBox.Show("Voud devez sélectionner un album");
            }
        }
        public void AddTune()
        {
           //  Tune _newTune = new Tune() { title = "Test" };
           // testDataServices.LesTunes.Add(_newTune);
        }
        public void DeleteTune()
        {
            //TODO: Arranger le delete sur la bonne tune
            if (TuneViewSource.CurrentItem != null)
            {
                string messaBoxText = "Êtes-vous certain de vouloir supprimer cette musique de l'album?";
                string caption = "Vous êtes sur le point de détruire une musique de l'album";
                MessageBoxButton button = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messaBoxText, caption, button, icon);
                if (result == MessageBoxResult.OK)
                {
                   // Tune tune = (Tune)TuneViewSource.CurrentItem;
                    //testDataServices.LesTunes.Remove(tune);
                }
            }
            else
            {
                MessageBox.Show("Voud devez sélectionner une tune");
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
        public ICollectionView TuneViewSource
        {
            get => tuneViewSource;
            set
            {
                tuneViewSource = value;
                OnPropertyChanged();
            }
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
            var albums = await Playlist.getAllAlbumsAsync();
            AlbumlistViewSource = CollectionViewSource.GetDefaultView(albums);
        }

        private async void SetTuneListAsync()
        {
           // int id = ;
            //var tunes = await Playlist.getAllTuneAsync(id);
           // TuneViewSource = CollectionViewSource.GetDefaultView(tunes);
        }

    }
}
