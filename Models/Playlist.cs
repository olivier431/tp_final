using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

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
        public async void SetTunesAsync()
        {
            string type = isPlaylist ? "playlist" : "album";
            JsonObject jsonParams = new() { { nameof(id), id } };

            var response = Martha.ExecuteQueryAsync($"select-{type}-tunes", jsonParams);

            await response;
            var Result = response.Result;
            if (!Result.Success) throw new Exception();
            //if (!Result.Data.Any()) return;

            tunes = new();
            Result.Data.ToList().ForEach(json =>
                tunes.Add(new Tune(json.ToString()!))
            );
        }

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
        //public static async Task<ObservableCollection<Playlist>?> AddAlbumAsync(int userId, string title, string artist, string genre, string albumCover, int year)
        //{
        //    JsonObject jsonParams = new()
        //    {
        //        { nameof(user_id), userId },
        //        { nameof(title), title },
        //        { nameof(artist), artist },
        //        { nameof(genre), genre },
        //        { nameof(album_cover), albumCover },
        //        { nameof(year), year }
        //    };
        //    //TODO make query
        //    var Result = await Martha.ExecuteQueryAsync($"", jsonParams);
        //    if (!Result.Success || !Result.Data.Any())
        //    {
        //        MessageBox.Show("error while adding");
        //        return null; //erreur
        //    }

        //    //return ;
        //}

        public static async Task<Playlist?> GetPlaylistByIdAsync(int id)
        {
            JsonObject jsonParams = new()
            {{nameof(id),id}};

            var Result = await Martha.ExecuteQueryAsync("select-playlist", jsonParams);

            if (!Result.Success) throw new Exception();
            if (!Result.Data.Any()) throw new Exception();

            return new(Result.Data.FirstOrDefault()!.ToString()!);
        }

        public override string ToString() =>
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}
