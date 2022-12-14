using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace tp_final.Models
{
    public class Playlist : Model
    {
        // --------------------- Properties ---------------------
        public int id { get; set; }
        public int user_id { get; set; }
        public int isPublic { get; set; }
        public string title { get; set; }
        public int count { get; set; }
        public int length { get; set; }
        public int? isAlbum { get; set; }
        public string? artist { get; set; }
        public string? genre { get; set; }
        public string? album_cover { get; set; }
        public int? year { get; set; }



        // --------------------- Constructors ---------------------
        public Playlist() 
        { 
            
        }

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
            int? isAlbum,
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
        }



        // --------------------- Methods ---------------------
        public override string ToString() =>
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });


        public virtual ObservableCollection<Tune> MusicPlaylist { get; set; } = new ObservableCollection<Tune>();
    }
}
