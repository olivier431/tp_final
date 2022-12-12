using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using tp_final.Models;

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

        //[JsonConstructor]
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
        public override string ToString() =>
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });


        public static async void selectUser(string username, string pwd) {
            string jsonParams = String.Format("{\"username\": \"{0}\", \"pwd\": \"{1}\"}", username, pwd);


            Console.WriteLine(jsonParams);
            var Result = await Martha.ExecuteQuery("select-user", jsonParams);


            Console.WriteLine(Result.Data.ToList().FirstOrDefault().ToString());

            //{ "username": "admin", "pwd": "admin"}
        }
    }

    


}
