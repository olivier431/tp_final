using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using tp_final.Commands;
using tp_final.Models;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    class MainPlayerViewModel : BaseViewModel
    {
        //public int id { get; set; }
        //public int user_id { get; set; }
        //public int isPublic { get; set; }
        //public string title { get; set; }
        //public int count { get; set; }
        //public int length { get; set; }

        //ICollectionView & Variables
        ICollectionView playlistViewSource;

        private string title;
        private int length, isPublic, count;

        //Navigation
        private readonly NavigationStore navigationStore;

        //Nav Bar DelegateCommands
        public DelegateCommand GoToAdminCommand { get; set; }
        public DelegateCommand GoToAlbumCommand { get; set; }
        public DelegateCommand LogoutCommand { get; set; }

        //Playlist Button Bar DelegateCommands
        public DelegateCommand AddPlaylistCommand { get; private set; }
        public DelegateCommand UpdatePlaylistCommand { get; private set; }
        public DelegateCommand RemoveSongCommand { get; private set; }
        public DelegateCommand DeletePlaylistCommand { get; private set; }

        //Button Bar DelegateCommands
        public DelegateCommand ShuffleCommand { get; private set; }
        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand PreviousCommand { get; private set; }
        public DelegateCommand LikeCommand { get; private set; }

        public MainPlayerViewModel(NavigationStore _navigationStore)
        {
            User currentUser = (User)Application.Current.Properties["CurrentUser"];

            //CollectionView
            PlaylistViewSource = CollectionViewSource.GetDefaultView(currentUser.playlists);

            //Nav Bar DelegateCommands
            navigationStore = _navigationStore;
            GoToAdminCommand = new DelegateCommand(GoToAdmin);
            GoToAlbumCommand = new DelegateCommand(GoToAlbum);
            LogoutCommand = new DelegateCommand(Logout);

            //Playlist Button Bar DelegateCommands
            AddPlaylistCommand = new DelegateCommand(AddPlaylist);
            UpdatePlaylistCommand = new DelegateCommand(UpdatePlaylist);
            RemoveSongCommand = new DelegateCommand(RemoveSong);
            DeletePlaylistCommand = new DelegateCommand(DeletePlaylist);

            //Button Bar DelegateCommands
            ShuffleCommand = new DelegateCommand(Shuffle);
            PlayCommand = new DelegateCommand(Play);
            PauseCommand = new DelegateCommand(Pause);
            NextCommand = new DelegateCommand(Next);
            PreviousCommand = new DelegateCommand(Previous);
            LikeCommand = new DelegateCommand(Like);
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

        public void AddPlaylist()
        {
            AddPlaylistAsync();
        }

        public void UpdatePlaylist()
        {
            UpdatePlaylistAsync();
        }

        public void RemoveSong()
        {
            User currentUser = (User)Application.Current.Properties["CurrentUser"];
            Playlist playlist = (Playlist)PlaylistViewSource.CurrentItem;
            
            if (playlist != null)
            {
                if (playlist.id != 1)
                {
                    if (playlist.user_id == currentUser.id || currentUser.isAdmin == 1)
                    {
                        string messaBoxText = "Voulez-vous supprimer les morceaux?";
                        string caption = "Vous êtes sur le point de supprimer les morceaux de la playlist";
                        MessageBoxButton button = MessageBoxButton.YesNo;
                        MessageBoxImage icon = MessageBoxImage.Warning;
                        MessageBoxResult result = MessageBox.Show(messaBoxText, caption, button, icon);
                        if (result == MessageBoxResult.Yes)
                            foreach (var morceau in playlist.tunes)
                                if (morceau.user_id == currentUser.id || currentUser.isAdmin == 1)
                                    morceau.DeleteTune();
                        
                        playlistViewSource.Refresh();   
                    }
                    else
                    {
                        MessageBox.Show("You can only delete a playlist that you own");
                    }
                }
                else
                {
                    MessageBox.Show("You cannot delete this playlist");
                }
            }
            else
            {
                MessageBox.Show("You need to select a playlist");
            }
        }

        public void DeletePlaylist()
        {
            User currentUser = (User)Application.Current.Properties["CurrentUser"];
            Playlist playlist = (Playlist)PlaylistViewSource.CurrentItem;
            if (playlist != null)
            {
                if (playlist.id != 1)
                {
                    if (playlist.user_id == currentUser.id || currentUser.isAdmin == 1)
                    {
                        string messaBoxText = "Do you really wish to delete this playlist ?";
                        string caption = "You are about to delete a playlist";
                        MessageBoxButton button = MessageBoxButton.YesNo;
                        MessageBoxImage icon = MessageBoxImage.Warning;
                        MessageBoxResult result = MessageBox.Show(messaBoxText, caption, button, icon);
                        if (result == MessageBoxResult.Yes)
                        {
                            currentUser.playlists.Remove(playlist);
                            playlist.DeleteAsync();
                            playlistViewSource.Refresh();
                        }
                    }
                    else
                    {
                        MessageBox.Show("You can only delete a playlist that you own");
                    }
                }
                else
                {
                    MessageBox.Show("You cannot delete this playlist");
                }
            }
            else
            {
                MessageBox.Show("You need to select a playlist");
            }
        }

        public void Shuffle() { }
        public void Play() 
        {
            Playlist playlist = (Playlist)PlaylistViewSource.CurrentItem;

        }
        public void Pause() { }
        public void Next() { }
        public void Previous() { }
        public void Like() { }

        public string Title
        {
            get => title;
            set => title = value;
        }
        public int IsPublic
        {
            get => isPublic;
            set => isPublic = value;
        }
        public int Count
        {
            get => count;
            set => count = value;
        }
        public int Length
        {
            get => length;
            set => length = value;
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

        private async void AddPlaylistAsync()
        {
            User currentUser = (User)Application.Current.Properties["CurrentUser"];
            await currentUser.AddPlaylistAsync("", 0, 0, 0);
        }

        private async void UpdatePlaylistAsync()
        {
            Playlist playlist = (Playlist)PlaylistViewSource.CurrentItem;
            playlist.UpdatePlaylistAsync(playlist.title, playlist.isPublic, playlist.count, playlist.length, playlist.id);
            playlist.isAlbum = 0;
        }
    }
}
