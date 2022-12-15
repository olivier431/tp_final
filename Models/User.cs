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
        public int id { get; set; }
        public int isAdmin { get; set; }
        public string username { get; set; }
        public string pwd { get; set; }
        public string? email { get; set; }
        public DateTime? lastConnection { get; set; }

       

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
        }



        // --------------------- Methods ---------------------
        public static async Task<User> GetUserAsync(string username, string pwd)
        {
            JsonObject jsonParams = new JsonObject
            {
                { nameof(username), username },
                { nameof(pwd), pwd }
            };

            var Result = await Martha.ExecuteQueryAsync("select-user", jsonParams);
            if (!Result.Success || !Result.Data.Any()) return null; //erreur
            return new(Result.Data.ToList().FirstOrDefault()!.ToString()!);
        }
        public override string ToString() =>
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

    
    }
}
