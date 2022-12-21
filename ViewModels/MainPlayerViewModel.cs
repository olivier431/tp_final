using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using tp_final.Commands;
using tp_final.Models;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    class MainPlayerViewModel : BaseViewModel
    {
        //ICollectionView
        ICollectionView tuneViewSource;
        ICollectionView playlistViewSource;

        private string title, artist, genre, albumCover;
        private int year;

        //NavigationStore
        private readonly NavigationStore navigationStore;

        //Nav Bar DelegateCommands
        public DelegateCommand GoToAdminCommand { get; set; }
        public DelegateCommand GoToAlbumCommand { get; set; }
        public DelegateCommand LogoutCommand { get; set; }

        //Button Bar DelegateCommands
        public DelegateCommand ShuffleCommand { get; private set; }
        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand PreviousCommand { get; private set; }
        public DelegateCommand LikeCommand { get; private set; }


        //TestDataServices testDataServices = new TestDataServices();

        public MainPlayerViewModel(NavigationStore _navigationStore)
        {
            User currentUser = (User)Application.Current.Properties["CurrentUser"];
            //TuneViewSource = CollectionViewSource.GetDefaultView(testDataServices.LesTunes);
            //CollectionView
            PlaylistViewSource = CollectionViewSource.GetDefaultView(currentUser.playlists);

            //Navigation
            navigationStore = _navigationStore;
            GoToAdminCommand = new DelegateCommand(GoToAdmin);
            GoToAlbumCommand = new DelegateCommand(GoToAlbum);
            LogoutCommand = new DelegateCommand(Logout);

            //MessageBox.Show(Application.Current.Properties["CurrentUser"].ToString());
        }

        public void GoToAdmin()
        {
            User currentUser = (User)Application.Current.Properties["CurrentUser"];
            if (currentUser.isAdmin == 1)
            {
                navigationStore.CurrentViewModel = new AdminUserViewModel(navigationStore);
            }
            else
            {
                MessageBox.Show("you are not an admin!");
            }
        }

        public void GoToAlbum()
        {
            navigationStore.CurrentViewModel = new AlbumViewModel(navigationStore);
        }

        public void Logout()
        {
            navigationStore.CurrentViewModel = new LoginViewModel(navigationStore);
            Application.Current.Properties["CurrentUser"] = null;
        }

        public string Title
        {
            get => title;
            set => title = value;
        }
        public string Artist
        {
            get => artist;
            set => artist = value;
        }
        public string Genre
        {
            get => genre;
            set => genre = value;
        }
        public string AlbumCover
        {
            get => albumCover;
            set => albumCover = value;
        }
        public int Year
        {
            get => year;
            set => year = value;
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
