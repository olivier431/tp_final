using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace tp_final.Models
{
    public class User : Model
    {
        // --------------------- Properties ---------------------
        private bool notAdmin;

        public int id { get; set; }
        public int isAdmin
        {
            get => (notAdmin) ? 0 : 1;
            set => notAdmin = (value == 0);
        }
        public string username { get; set; }
        public string pwd { get; set; }
        public string? email { get; set; }
        public DateTime? lastConnection { get; set; }

        public ObservableCollection<Playlist> albums { get; private set; }
        public ObservableCollection<Playlist> playlists { get; private set; }



        // --------------------- Constructors ---------------------
        public User() { }

        public User(string json) :
        this(JsonSerializer.Deserialize<User>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!)
        { }

        public User(User user) :
        this(
            user.id,
            user.isAdmin,
            user.username,
            user.pwd,
            user.email,
            user.lastConnection
        )
        { }

        User(
            int id,
            int isAdmin,
            string username,
            string pwd,
            string? email,
            DateTime? lastConnection
        )
        {
            this.id = id;
            this.isAdmin = isAdmin;
            this.username = username;
            this.pwd = pwd;
            this.email = email;
            this.lastConnection = lastConnection;

            albums = new();
            //SetAlbumsAsync();
        }



        // --------------------- Methods ---------------------
        public async void SetAlbumsAsync()
        {
            string type = notAdmin ? "-user" : "";
            JsonObject jsonParams = new() { { nameof(id), id } };

            var response = Martha.ExecuteQueryAsync($"select{type}-albums", jsonParams);

            await response;
            var Result = response.Result;
            if (!Result.Success) throw new Exception();
            //if (!Result.Data.Any()) return;

            albums = new();
            Result.Data.ToList().ForEach(json =>
                albums.Add(new Playlist(json.ToString()!))
            );
        }

        public async Task<ObservableCollection<Playlist>> AddAlbumAsync(
            string title,
            string artist,
            string genre,
            string? album_cover,
            int year)
        {
            var album = await Playlist.AddAlbumAsync(id, title, artist, genre, album_cover, year);

            if (album == null) return null;

            albums.Add(album);
            return albums;
        }



        public async void SetPlaylistsAsync()
        {
            string type = notAdmin ? "-user" : "";
            JsonObject jsonParams = new() { { nameof(id), id } };

            var response = Martha.ExecuteQueryAsync($"select{type}-playlists", jsonParams);

            await response;
            var Result = response.Result;
            if (!Result.Success) throw new Exception();
            //if (!Result.Data.Any()) return;

            playlists = new();
            Result.Data.ToList().ForEach(json =>
                playlists.Add(new Playlist(json.ToString()!))
            );
        }

        public async Task<ObservableCollection<Playlist>> AddAlbumAsync(string title)
        {
            var playlist = await Playlist.AddPlaylistAsync(id, title);

            if (playlist == null) return null;

            playlists.Add(playlist);
            return playlists;
        }



        public override string ToString() =>
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

        public static async Task<ObservableCollection<User>?> GetAllUsersAsync()
        {
            var Result = await Martha.ExecuteQueryAsync("select-users");

            if (!Result.Success) throw new Exception();
            if (!Result.Data.Any()) throw new Exception();

            ObservableCollection<User> users = new();
            Result.Data.ToList().ForEach(json =>
                users.Add(new User(json.ToString()!))
            );

            return users;
        }

        public static async Task<User?> GetUserAsync(string username, string pwd)
        {
            JsonObject jsonParams = new()
            {
                { nameof(username), username },
                { nameof(pwd), pwd }
            };

            var Result = await Martha.ExecuteQueryAsync("select-user", jsonParams);
            if (!Result.Success || !Result.Data.Any()) return null; //erreur

            User user = new(Result.Data.ToList().FirstOrDefault()!.ToString()!);
            user.SetAlbumsAsync();
            return user;
        }

        public static async Task<User?> AddUserAsync(string username, string pwd, string email)
        {
            JsonObject jsonParams = new()
            {
                { nameof(username), username },
                { nameof(pwd), pwd },
                { nameof(email), email }
            };

            var Result = await Martha.ExecuteQueryAsync($"insert-user", jsonParams);
            if (!Result.Success || !Result.Data.Any()) return null; //erreur
            return new(Result.Data.ToList().FirstOrDefault()!.ToString()!);
        }

        public static async Task<bool> UpdateUserAsync(string username, string email, string pwd, int id)
        {
            JsonObject jsonParams = new()
            {
                { nameof(username), username },
                { nameof(email), email },
                { nameof(pwd), pwd },
                { nameof(id), id }
            };

            var Result = await Martha.ExecuteQueryAsync($"update-user", jsonParams);
            return Result.Success;
        }

        public static async Task DeleteUserAsync(int id)
        {
            JsonObject jsonParams = new() { { nameof(id), id } };

            var Result = await Martha.ExecuteQueryAsync($"delete-user", jsonParams);
            if (!Result.Success) return;
        }
    }
}
