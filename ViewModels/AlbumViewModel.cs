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
    public class AlbumViewModel : BaseViewModel
    {
        ICollectionView tuneViewSource;
        ICollectionView playlistViewSource;

        //private Tune tune = new Tune();

        public DelegateCommand GoToAdminCommand { get; set; }
        public DelegateCommand GoToMainPlayerCommand { get; set; }

        public DelegateCommand AddAlbumCommand { get; private set; }
        public DelegateCommand DeleteAlbumCommand { get; private set; }
        public DelegateCommand AddTuneCommand { get; private set; }
        public DelegateCommand DeleteTuneCommand { get; private set; }

        TestDataServices testDataServices = new TestDataServices();
        private readonly NavigationStore navigationStore;

        public AlbumViewModel(NavigationStore _navigationStore) {

            TuneViewSource = CollectionViewSource.GetDefaultView(testDataServices.LesTunes);
            PlaylistViewSource = CollectionViewSource.GetDefaultView(testDataServices.LesPlaylists);

            AddAlbumCommand = new DelegateCommand(AddAlbum);
            DeleteAlbumCommand = new DelegateCommand(DeleteAlbum);
            AddTuneCommand = new DelegateCommand(AddTune);
            DeleteTuneCommand = new DelegateCommand(DeleteTune);

            //if (PlaylistViewSource.CurrentItem != null)
            //{
            //    Tune tune = (Tune)PlaylistViewSource.CurrentItem;
            //}

            navigationStore = _navigationStore;
            GoToAdminCommand = new DelegateCommand(GoToAdmin);
            GoToMainPlayerCommand = new DelegateCommand(GoToMainPlayer);
        }

        public void AddAlbum()
        {
            //string messaBoxText = "Êtes-vous certain de vouloir supprimer ce user?";
            //string caption = "Vous êtes sur le point de détruire un user";
            //MessageBoxButton button = MessageBoxButton.OKCancel;
            //MessageBoxImage icon = MessageBoxImage.Warning;
            //MessageBoxResult result = MessageBox.Show(messaBoxText, caption, button, icon);
            Playlists _newAlbum = new Playlists() { Title = "Test"};
            //.Add(_newAlbum);
            
        }

        public void DeleteAlbum()
        {
            string messaBoxText = "Êtes-vous certain de vouloir supprimer cet album?";
            string caption = "Vous êtes sur le point de détruire un album";
            MessageBoxButton button = MessageBoxButton.OKCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(messaBoxText, caption, button, icon);
        }
        public void AddTune()
        {
        
        }
        public void DeleteTune()
        {
            string messaBoxText = "Êtes-vous certain de vouloir supprimer cette musique de l'album?";
            string caption = "Vous êtes sur le point de détruire une musique de l'album";
            MessageBoxButton button = MessageBoxButton.OKCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(messaBoxText, caption, button, icon);
        }




        public void GoToAdmin()
        {
            navigationStore.CurrentViewModel = new AdminUserViewModel(navigationStore);
        }

        public void GoToMainPlayer()
        {
            navigationStore.CurrentViewModel = new MainPlayerViewModel(navigationStore);
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
        public ICollectionView PlaylistViewSource
        {
            get => playlistViewSource;
            set
            {
                playlistViewSource = value;
                OnPropertyChanged();
            }
        }

    }
}
