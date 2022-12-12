using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;


namespace tp_final.Services
{
    public class MarthaProcessor
    {
        private static readonly MarthaProcessor instance = new MarthaProcessor();
        private readonly HttpClient httpClient = new HttpClient();

        IConfiguration Configuration;

        static MarthaProcessor()
        {

        }

        private MarthaProcessor()
        {
            doConfig();

        }

        /// GetInstance
        public static MarthaProcessor Instance => instance;

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        void doConfig()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json",
                optional: true,
                reloadOnChange: true);

            builder.AddUserSecrets<MarthaProcessor>();

            Configuration = builder.Build();

            var user = Configuration["username"];
            var pw = Configuration["pwd"];
            var userpw = Base64Encode($"{user}:{pw}");

            // Workaround for MacOS...
            var baseAddress = Configuration["baseAddress"] ?? "http://martha.jh.shawinigan.info";

            var uri = new Uri(baseAddress);

            httpClient.BaseAddress = uri;
            httpClient.DefaultRequestHeaders.Add("auth", userpw);
        }

        /// <summary>
        /// Execute a query
        /// </summary>
        /// <param name="queryName">Nom du Query sur Martha</param>
        /// <param name="param">Format JSON {"nomParam" : "valeurParam" [, ...]}</param>
        /// <returns>Une MarthaResponse</returns>
        /// <exception cref="Exception"></exception>
        public async Task<MarthaResponse> ExecuteQueryAsync(string queryName, string param = "{}")
        {
            var url = $"queries/{queryName}/execute";
            var httpContent = new StringContent(param, Encoding.UTF8, "application/json");


            using (var response = await httpClient.PostAsync(url, httpContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();

                    var result = JsonSerializer.Deserialize<MarthaResponse>(stringContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return result;

                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Execute a query
        /// </summary>
        /// <param name="queryName">Nom du Query sur Martha</param>
        /// <param name="jso">Objet JSON {"nomParam" : "valeurParam" [, ...]}</param>
        /// <returns>Une MarthaResponse</returns>
        /// <exception cref="Exception"></exception>
        public async Task<MarthaResponse> ExecuteQueryAsync(string queryName, JsonObject jso) => await ExecuteQueryAsync(queryName, jso.ToString());
    }
}
