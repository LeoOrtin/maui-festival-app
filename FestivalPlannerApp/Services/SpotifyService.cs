using System.Text.Json;

namespace FestivalPlannerApp.Services
{
    public class SpotifyService : ISpotifyService
    {
        const string clientId = Secrets.ClientId;
        const string clientSecret = Secrets.ClientSecret;
        const string redirectUri = "https://myapp/";
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;
        public SpotifyService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

    }
}
