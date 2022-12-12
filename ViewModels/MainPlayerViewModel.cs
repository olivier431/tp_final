using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using tp_final.Commands;
using tp_final.Services;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    class MainPlayerViewModel : BaseViewModel
    {
        ICollectionView tuneViewSource;
        ICollectionView playlistViewSource;
        private readonly NavigationStore navigationStore;
        public DelegateCommand GoToAdminCommand { get; set; }
        public DelegateCommand GoToAlbumCommand { get; set; }

        TestDataServices testDataServices = new TestDataServices();

        public MainPlayerViewModel(NavigationStore _navigationStore)
        {
            TuneViewSource = CollectionViewSource.GetDefaultView(testDataServices.LesTunes);
            PlaylistViewSource = CollectionViewSource.GetDefaultView(testDataServices.LesPlaylists);

            navigationStore = _navigationStore;
            GoToAdminCommand = new DelegateCommand(GoToAdmin);
            GoToAlbumCommand = new DelegateCommand(GoToAlbum);
        }

        public void GoToAdmin()
        {
            navigationStore.CurrentViewModel = new AdminUserViewModel(navigationStore);
        }

        public void GoToAlbum()
        {
            navigationStore.CurrentViewModel = new AlbumViewModel(navigationStore);
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
