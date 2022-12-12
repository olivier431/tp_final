using System.Text.Json;
using System.Text.Json.Serialization;

namespace tp_final.Models
{
    public class Tune : Model
    {
        // --------------------- Properties ---------------------
        public int id { get; set; }
        public int user_id { get; set; }
        public int album_id { get; set; }
        public int album_ord { get; set; }
        public string title { get; set; }
        public string artist { get; set; }
        public string? genre { get; set; }
        public string? filepath { get; set; }
        public int length { get; set; }
        public int? year { get; set; }



        // --------------------- Constructors ---------------------
        public Tune(string json) :
        this(JsonSerializer.Deserialize<Tune>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!)
        { }

        public Tune(Tune tune) : 
        this(
            tune.id,
            tune.user_id,
            tune.album_id,
            tune.album_ord,
            tune.title,
            tune.artist,
            tune.genre,
            tune.filepath,
            tune.length,
            tune.year
        )
        { }

        [JsonConstructor]
        public Tune(
            int id,
            int user_id,
            int album_id,
            int album_ord,
            string title,
            string artist,
            string? genre,
            string? filepath,
            int length,
            int? year
        )
        {
            this.id = id;
            this.user_id = user_id;
            this.album_id = album_id;
            this.album_ord = album_ord;
            this.title = title;
            this.artist = artist;
            this.genre = genre;
            this.filepath = filepath;
            this.length = length;
            this.year = year;
        }



        // --------------------- Methods ---------------------
        public override string ToString() =>
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}
