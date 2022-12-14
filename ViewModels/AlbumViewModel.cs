using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using tp_final.Commands;
using tp_final.Models;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    public class AlbumViewModel : BaseViewModel
    {
        //CollectionView
        ICollectionView albumlistViewSource;
        ICollectionView tunelistViewSource;
        private ObservableCollection<Tune> tunesunknown;
        public Tune Selectedtune { get; set; }
        //Delegate Command
        public DelegateCommand GoToAdminCommand { get; set; }
        public DelegateCommand GoToMainPlayerCommand { get; set; }
        public DelegateCommand LogoutCommand { get; set; }
        public DelegateCommand AddAlbumCommand { get; private set; }
        public DelegateCommand DeleteAlbumCommand { get; private set; }
        public DelegateCommand UpdateAlbumCommand { get; private set; }
        public DelegateCommand OrderAlbumCommand { get; private set; }
        public DelegateCommand ShuffleCommand { get; private set; }
        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand PreviousCommand { get; private set; }
        public DelegateCommand LikeCommand { get; private set; }
        public DelegateCommand AddTuneCommand { get; private set; }
        public DelegateCommand RemoveTuneCommand { get; private set; }
        //Navigation
        private readonly NavigationStore navigationStore;
       
        public AlbumViewModel(NavigationStore _navigationStore)
        {
            User CurUser = (User)Application.Current.Properties["CurrentUser"];

            //var temp = Application.Current.Properties["CurrentUserId"];
            //MessageBox.Show(temp.ToString());
            
            //CollectionView
            AlbumlistViewSource = CollectionViewSource.GetDefaultView(CurUser.albums);
           // TunelistViewSource = CollectionViewSource.GetDefaultView(CurUser.albums);
            SetUnknownListAsync();
            //Add
            AddAlbumCommand = new DelegateCommand(AddAlbum);

            //Delete
            DeleteAlbumCommand = new DelegateCommand(DeleteAlbum);

            OrderAlbumCommand = new DelegateCommand(OrderAlbum);

            UpdateAlbumCommand = new DelegateCommand(UpdateAlbum);
            AddTuneCommand = new DelegateCommand(AddTune);
            RemoveTuneCommand = new DelegateCommand(RemoveTune);

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
            LogoutCommand = new DelegateCommand(Logout);
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
        public ICollectionView TunelistViewSource
        {
            get => tunelistViewSource;
            set
            {
                tunelistViewSource = value;
                OnPropertyChanged();
            }
        }
        public void GoToAdmin()
        {
            User CurUser = (User)Application.Current.Properties["CurrentUser"];
            if (CurUser.isAdmin == 1)
            {
                navigationStore.CurrentViewModel = new AdminUserViewModel(navigationStore);
            }
            else
            {
                MessageBox.Show("you are not an admin!");
            }
        }
        public void GoToMainPlayer()
        {
            navigationStore.CurrentViewModel = new MainPlayerViewModel(navigationStore);
        }
        public void Logout()
        {
            navigationStore.CurrentViewModel = new LoginViewModel(navigationStore);
            Application.Current.Properties["CurrentUser"] = null;
        }
        public void OrderAlbum() 
        {
            Playlist playlist = (Playlist)AlbumlistViewSource.CurrentItem;
            Random rand = new Random();
            playlist.EditOrderAsync(playlist.id, rand.Next(0, playlist.tunes.Count()));
        }
        public void Shuffle() 
        {
            //TODO: Mettre le play après le shuffle
            Playlist playlist = (Playlist)AlbumlistViewSource.CurrentItem;
            playlist.ShuffleOrderAsync();
        }
        public void Play() { }
        public void Pause() { }
        public void Next() { }
        public void Previous() { }
        public void Like() { }

        public void UpdateAlbum() 
        {
            UpdateAlbumListAsync();
        }
        public void AddAlbum()
        {
            AddAlbumListAsync();
        }
        public void DeleteAlbum()
        {
            User CurUser = (User)Application.Current.Properties["CurrentUser"];
            Playlist playlist = (Playlist)AlbumlistViewSource.CurrentItem;
            //Tune tunes = (Tune)TunelistViewSource.CurrentItem;
            if (playlist != null)
            {
                if (playlist.title != "Unknown Album" && playlist.id != 1)
                {
                    if (playlist.user_id == CurUser.id || CurUser.isAdmin == 1)
                    {
                        string messaBoxText = "Êtes-vous certain de vouloir supprimer cet album?";
                        string caption = "Vous êtes sur le point de supprimer un album";
                        MessageBoxButton button = MessageBoxButton.YesNo;
                        MessageBoxImage icon = MessageBoxImage.Warning;
                        MessageBoxResult result = MessageBox.Show(messaBoxText, caption, button, icon);
                        if (result == MessageBoxResult.Yes)
                        {
                            messaBoxText = "Voulez-vous supprimer les morceaux?";
                            caption = "Vous êtes sur le point de supprimer les morceaux de l'album";
                            button = MessageBoxButton.YesNo;
                            icon = MessageBoxImage.Warning;
                            result = MessageBox.Show(messaBoxText, caption, button, icon);
                            if (result == MessageBoxResult.Yes)
                            {
                                foreach (var morceau in playlist.tunes)
                                    if (morceau.user_id == CurUser.id || CurUser.isAdmin == 1)
                                    {
                                        morceau.DeleteTune();
                                    }
                                    else
                                    {
                                        morceau.UpdateUnknown();
                                        tunesunknown.Add(morceau);
                                    }
                            }
                            else
                            {
                                foreach(var morceau in playlist.tunes)
                                {
                                    tunesunknown.Add(morceau);
                                }
                            }
                            CurUser.albums.Remove(playlist);
                            playlist.DeleteAsync();
                            Playlist Unknown = CurUser.albums.First(x => x.title.Equals("Unknown Album"));
                            Unknown?.SetTunesAsync();
                            albumlistViewSource.Refresh();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vous devez supprimer un album qui vous appartient");
                    }
                }
                else
                {
                    MessageBox.Show("Vous ne pouvez pas supprimer cet album");
                }
            }
            else
            {
                MessageBox.Show("Vous devez sélectionner un album");
            }
        }
        public void AddTune()
        {
            Playlist playlist = (Playlist)AlbumlistViewSource.CurrentItem;
            Tune tunes = (Tune)TunelistViewSource.CurrentItem;
            if (AlbumlistViewSource.CurrentItem != null)
            {
                tunes.album_id = playlist.id;
                tunes.UpdateTune();
                playlist.length += tunes.length;
                playlist.tunes.Add(tunes);
                tunesunknown.Remove(tunes);
                albumlistViewSource.Refresh();
                TunelistViewSource.Refresh();
            }
        }
        public void RemoveTune() 
        {
            Playlist playlist = (Playlist)AlbumlistViewSource.CurrentItem;
            // if (obj is Tune)
            // {
            //  Tune tune = (Tune)obj;
            Selectedtune.UpdateUnknown();
            playlist.tunes.Remove(Selectedtune);
            playlist.length -= Selectedtune.length;
            tunesunknown.Add(Selectedtune);
            albumlistViewSource.Refresh();
            // }

            // Tune tunes = (Tune)AlbumlistViewSource.CurrentItem;
            // if (AlbumlistViewSource.CurrentItem != null)
            // {
            // if (obj is tunes)
            // {

            // }
            //MessageBox.Show(tunes.ToString)
            //  Tune tune = (Tune)playlist.tunes;
            //  playlist.id = 1;
            //  tunes.UpdateUnknown();
            ////  playlist.length -= playlist.tunes;
            //  playlist.tunes.Remove(tunes);
            //  tunesunknown.Add(tunes);
            //  albumlistViewSource.Refresh();
            //  albumlistViewSource.Refresh();
            // }
        }

        public void UpdateAlbumListAsync() 
        {
            Playlist playlist = (Playlist)AlbumlistViewSource.CurrentItem;
            playlist.UpdateAlbumAsync(playlist.title, playlist.artist, playlist.genre, playlist.album_cover, playlist.year.Value, playlist.id);
            playlist.isAlbum = 1;
        }
        private async void AddAlbumListAsync()
        {
            
            User CurUser = (User)Application.Current.Properties["CurrentUser"];
            await CurUser.AddAlbumAsync("", "", "", "", 0);
        }
        private async void SetUnknownListAsync()
        {
            tunesunknown = await Playlist.GetTuneAlbumsUnknownAsync();
            if (tunesunknown == null)
            {

            }
            else
            {
                TunelistViewSource = CollectionViewSource.GetDefaultView(tunesunknown);
            }
        }
    }
}
