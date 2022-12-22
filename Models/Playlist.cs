using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace tp_final.Models
{
    public class Playlist : Model
    {
        // --------------------- Properties ---------------------
        private bool isPlaylist;

        public int id { get; set; }
        public int user_id { get; set; }
        public int isPublic { get; set; }
        public string title { get; set; }
        public int count { get; set; }
        public int length { get; set; }
        public int isAlbum
        {
            get => (isPlaylist) ? 0 : 1;
            set => isPlaylist = (value == 0);
        }
        public string? artist { get; set; }
        public string? genre { get; set; }
        public string? album_cover { get; set; }
        public int? year { get; set; }

        public ObservableCollection<Tune> tunes { get; private set; }



        // --------------------- Constructors ---------------------
        public Playlist(string json) :
        this(JsonSerializer.Deserialize<Playlist>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!)
        { }

        public Playlist(Playlist playlist) :
        this(
            playlist.id,
            playlist.user_id,
            playlist.isPublic,
            playlist.title,
            playlist.count,
            playlist.length,
            playlist.isAlbum,
            playlist.artist,
            playlist.genre,
            playlist.album_cover,
            playlist.year
        )
        { }

        [JsonConstructor]
        public Playlist(
            int id,
            int user_id,
            int isPublic,
            string title,
            int count,
            int length,
            int isAlbum,
            string? artist,
            string? genre,
            string? album_cover,
            int? year
        )
        {
            this.id = id;
            this.user_id = user_id;
            this.isPublic = isPublic;
            this.count = count;
            this.length = length;
            this.isAlbum = isAlbum;
            this.title = title;
            this.artist = artist;
            this.genre = genre;
            this.album_cover = album_cover;
            this.year = year;

            tunes = new();
            SetTunesAsync();
        }



        // --------------------- Methods ---------------------
        public async void SetTunesAsync() => tunes = await SetTunesAsync(isPlaylist, id);

        public async Task<ObservableCollection<Tune>?> AddTuneAsync(
            string title,
            string artist,
            string? genre,
            string? filepath,
            int length,
            int? year)
        {
            var tune = await Tune.AddTuneAsync(user_id, id, title, artist, genre, filepath, length, year);

            if (tune == null) return null;

            tunes.Add(tune);
            return tunes;
        }

        public async void EditOrderAsync(int OLD_ord, int NEW_ord)
        {
            JsonObject jsonParams = new()
            {
                { nameof(OLD_ord), OLD_ord },
                { nameof(NEW_ord), NEW_ord },
                { nameof(id), id }
            };

            var Result = await Martha.ExecuteQueryAsync("update-playlist-ord", jsonParams);
            if (!Result.Success) throw new Exception();

            tunes.Move(OLD_ord, NEW_ord);
        }

        public async void ShuffleOrderAsync()
        {
            JsonObject jsonParams = new() { { nameof(id), id } };

            var Result = await Martha.ExecuteQueryAsync("update-playlist-ord-shuffle", jsonParams);
            if (!Result.Success || (int)Result.Data.First() == 0) throw new Exception();

            SetTunesAsync();
        }

        public void DeleteAsync() => DeleteAsync(id);

        public override string ToString() =>
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });



        // --------------------- static Methods ---------------------
        public static async Task<ObservableCollection<Tune>> SetTunesAsync(bool isPlaylist, int id)
        {
            string type = isPlaylist ? "playlist" : "album";
            JsonObject jsonParams = new() { { nameof(id), id } };

            var Result = await Martha.ExecuteQueryAsync($"select-{type}-tunes", jsonParams);

            if (!Result.Success) throw new Exception();
            //if (!Result.Data.Any()) return;

            ObservableCollection<Tune> tunes = new();
            Result.Data.ToList().ForEach(json =>
                tunes.Add(new Tune(json.ToString()!))
            );
            return tunes;
        }

        public static async void DeleteAsync(int id)
        {
            JsonObject jsonParams = new() { { nameof(id), id } };

            var Result = await Martha.ExecuteQueryAsync($"delete-playlist", jsonParams);
            if (!Result.Success) throw new Exception();
        }



        // --------------------- Album Methods ---------------------
        public static async Task<ObservableCollection<Playlist>?> GetAllAlbumsAsync()
        {
            var Result = await Martha.ExecuteQueryAsync("select-albums");

            if (!Result.Success) throw new Exception();
            if (!Result.Data.Any()) throw new Exception();

            ObservableCollection<Playlist> albums = new();
            Result.Data.ToList().ForEach(json =>
                albums.Add(new Playlist(json.ToString()!))
            );

            return albums;
        }

        public static async Task<ObservableCollection<Playlist>?> GetTuneAlbumsUnknownAsync()
        {
            var Result = await Martha.ExecuteQueryAsync("select-albums-unknown");

            if (!Result.Success) throw new Exception();
            if (!Result.Data.Any()) throw new Exception();

            ObservableCollection<Playlist> albums = new();
            Result.Data.ToList().ForEach(json =>
                albums.Add(new Playlist(json.ToString()!))
            );

            return albums;
        }

        public static async Task<Playlist?> AddAlbumAsync(
            int user_id,
            string title,
            string artist,
            string genre,
            string? album_cover,
            int year)
        {
            JsonObject jsonParams = new()
            {
                { nameof(user_id), user_id },
                { nameof(title), title },
                { nameof(artist), artist },
                { nameof(genre), genre },
                { nameof(album_cover), album_cover },
                { nameof(year), year }
            };

            var Result = await Martha.ExecuteQueryAsync($"insert-album", jsonParams);
            if (!Result.Success || !Result.Data.Any()) return null; //erreur // MessageBox.Show("error while adding");
            return new(Result.Data.ToList().FirstOrDefault()!.ToString()!);
        }



        // --------------------- Playlist Methods ---------------------
        public static async Task<Playlist?> GetPlaylistByIdAsync(int id)
        {
            JsonObject jsonParams = new()
            {{nameof(id),id}};

            var Result = await Martha.ExecuteQueryAsync("select-playlist", jsonParams);

            if (!Result.Success) throw new Exception();
            if (!Result.Data.Any()) throw new Exception();

            return new(Result.Data.FirstOrDefault()!.ToString()!);
        }

        public static async Task<Playlist?> AddPlaylistAsync(int user_id, string title)
        {
            JsonObject jsonParams = new()
            {
                { nameof(user_id), user_id },
                { nameof(title), title }    

            };

            var Result = await Martha.ExecuteQueryAsync($"insert-playlist", jsonParams);
            if (!Result.Success || !Result.Data.Any()) return null; //erreur
            return new(Result.Data.ToList().FirstOrDefault()!.ToString()!);
        }

        public static async Task<Playlist?> AddPlaylistAsync2(int user_id, string title, int isPublic, int count, int length)
        {
            JsonObject jsonParams = new()
            {
                { nameof(user_id), user_id },
                { nameof(title), title },
                { nameof(isPublic), isPublic },
                { nameof(count), count },
                { nameof(length), length }

            };

            var Result = await Martha.ExecuteQueryAsync($"insert-playlist", jsonParams);
            if (!Result.Success || !Result.Data.Any()) return null; //erreur
            return new(Result.Data.ToList().FirstOrDefault()!.ToString()!);
        }
    }
}
