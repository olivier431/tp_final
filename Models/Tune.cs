using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace tp_final.Models
{
    public class Tune : Model
    {
        // --------------------- Properties ---------------------
        public int id { get; set; }
        public int user_id { get; set; }
        public int album_id { get; set; }
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
            this.title = title;
            this.artist = artist;
            this.genre = genre;
            this.filepath = filepath;
            this.length = length;
            this.year = year;
        }



        // --------------------- Methods ---------------------
        public void DeleteTune() => DeleteTuneAsync(id);

        public void UpdateTune() => UpdateTuneAsync(album_id, id);

        public void UpdateUnknown() => UpdateUnknownAsync(id);

        public override string ToString() =>
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });



        // --------------------- static Methods ---------------------
        public static async Task<Tune?> GetTuneByIdAsync(int id)
        {
            JsonObject jsonParams = new JsonObject
            {
                { nameof(id), id }
            };

            var Result = await Martha.ExecuteQueryAsync("select-tune", jsonParams);
            if (!Result.Success || !Result.Data.Any()) return null; //erreur
            return new(Result.Data.FirstOrDefault()!.ToString()!);
        }

        public static async Task<Tune?> AddTuneAsync(
            int user_id,
            int album_id,
            string title,
            string artist,
            string? genre,
            string? filepath,
            int length,
            int? year)
        {
            JsonObject jsonParams = new()
            {
                { nameof(user_id), user_id },
                { nameof(album_id), album_id },
                { nameof(title), title },
                { nameof(artist), artist },
                { nameof(genre), genre },
                { nameof(filepath), filepath },
                { nameof(length), length },
                { nameof(year), year }
            };

            var Result = await Martha.ExecuteQueryAsync($"insert-tune", jsonParams);
            if (!Result.Success || !Result.Data.Any()) return null; //erreur
            return new(Result.Data.ToList().FirstOrDefault()!.ToString()!);
        }

        public static async void DeleteTuneAsync(int id)
        {
            JsonObject jsonParams = new() { { nameof(id), id } };

            var Result = await Martha.ExecuteQueryAsync($"delete-tune", jsonParams);
            if (!Result.Success) return;
        }

        public static async void UpdateTuneAsync(int album_id, int id)
        {
            JsonObject jsonParams = new()
            {
                { nameof(album_id), album_id },
                { nameof(id), id }
            };

            var Result = await Martha.ExecuteQueryAsync($"update-tune-album", jsonParams);
            if (!Result.Success) return;
        }

        public static async void UpdateUnknownAsync( int id)
        {
            JsonObject jsonParams = new()
            {
                { nameof(id), id }
            };

            var Result = await Martha.ExecuteQueryAsync($"update-tune-unknown", jsonParams);
            if (!Result.Success) return;
        }


    }
}
