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
    public class AlbumViewModel : BaseViewModel
    {
        ICollectionView tuneViewSource;
        ICollectionView playlistViewSource;

        public DelegateCommand GoToAdminCommand { get; set; }

        TestDataServices testDataServices = new TestDataServices();
        private readonly NavigationStore navigationStore;
        
        public AlbumViewModel(NavigationStore _navigationStore) {

            TuneViewSource = CollectionViewSource.GetDefaultView(testDataServices.LesTunes);
            PlaylistViewSource = CollectionViewSource.GetDefaultView(testDataServices.LesPlaylists);

            navigationStore = _navigationStore;
            GoToAdminCommand = new DelegateCommand(GoToAdmin);
        }
        public void GoToAdmin()
        {
            navigationStore.CurrentViewModel = new AdminUserViewModel(navigationStore);
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
