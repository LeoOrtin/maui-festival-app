using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using FestivalPlannerApp.Models;

namespace FestivalPlannerApp.Services;

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
    public async Task<bool> Authenticate()
    {
        Token? Token;
        Uri uri = new($"https://accounts.spotify.com/api/token");
        string? authorizationCode = await GetAuthorizationCode();
        string codeVerifier = Preferences.Get("code_verifier", string.Empty);
        if (string.IsNullOrEmpty(authorizationCode))
        {
            Debug.WriteLine("Authorization code is null.");
            return false;
        }
        var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", authorizationCode),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("code_verifier", codeVerifier)
            ]);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        HttpRequestMessage request = new(HttpMethod.Post, uri)
        {
            Content = content
        };
        try
        {
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Token = JsonSerializer.Deserialize<Token>(responseContent, _serializerOptions);
                if (Token != null)
                {
                    Preferences.Set("access_token", Token.Access_token);
                    Preferences.Set("refresh_token", Token.Refresh_token);
                }
                return true;
            }
            else
            {
                Debug.WriteLine("Failed to get access token.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            return false;
        }
    }
    public bool IsSignedIn()
    {
        return Preferences.ContainsKey("access_token");
    }
    public void SignOut()
    {
        Preferences.Remove("access_token");
        Preferences.Remove("refresh_token");
    }
    public async Task<User> GetUser()
    {
        User? user = new();
        Uri uri = new($"https://api.spotify.com/v1/me");
        HttpRequestMessage request = new(HttpMethod.Get, uri);
        string accessToken = Preferences.Get("access_token", string.Empty);
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        try
        {
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshToken();
                accessToken = Preferences.Get("access_token", string.Empty);
                HttpRequestMessage newRequest = new(HttpMethod.Get, uri);
                newRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                response = await _client.SendAsync(newRequest);
            }
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                user = JsonSerializer.Deserialize<User>(content, _serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"\tERROR {0}", ex.Message);
        }
        return user ?? new User();
    }
    public async Task<List<Artist>> SearchArtists(string query)
    {
        ArtistsResult? result = new();
        Uri uri = new($"https://api.spotify.com/v1/search?q={query}&type=artist&limit=10");
        HttpRequestMessage request = new(HttpMethod.Get, uri);
        string accessToken = Preferences.Get("access_token", string.Empty);
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        try
        {
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshToken();
                accessToken = Preferences.Get("access_token", string.Empty);
                HttpRequestMessage newRequest = new(HttpMethod.Get, uri);
                newRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                response = await _client.SendAsync(newRequest);
            }
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                JsonDocument jsonDocument = JsonDocument.Parse(content);
                JsonElement root = jsonDocument.RootElement;
                if (root.TryGetProperty("artists", out JsonElement artists))
                {
                    result = JsonSerializer.Deserialize<ArtistsResult>(artists.GetRawText(), _serializerOptions);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"\tERROR {0}", ex.Message);
        }
        return result?.Items ?? [];
    }
    public async Task<List<Artist>> GetTopArtists(string frame, int limit)
    {
        ArtistsResult? result = new();
        List<Artist>? topArtists = [null];
        Uri uri = new($"https://api.spotify.com/v1/me/top/artists?time_range={frame}&limit={limit}");
        HttpRequestMessage request = new(HttpMethod.Get, uri);
        string accessToken = Preferences.Get("access_token", string.Empty);
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        try
        {
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshToken();
                accessToken = Preferences.Get("access_token", string.Empty);
                HttpRequestMessage newRequest = new(HttpMethod.Get, uri);
                newRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                response = await _client.SendAsync(newRequest);
            }
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<ArtistsResult>(content, _serializerOptions) ?? new();
                topArtists = new List<Artist>(result.Items);
                while (result.Next != null)
                {
                    uri = new(result.Next);
                    request = new(HttpMethod.Get, uri);
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    response = await _client.SendAsync(request);
                    content = await response.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<ArtistsResult>(content, _serializerOptions) ?? new();
                    foreach (Artist artist in result.Items)
                    {
                        topArtists.Add(artist);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"\tERROR {0}", ex.Message);
        }
        return topArtists ?? [];
    }
    public async Task<List<Track>> GetTopTracks(int limit)
    {
        TracksResult? result = new();
        Uri uri = new($"https://api.spotify.com/v1/me/top/tracks?limit={limit}");
        HttpRequestMessage request = new(HttpMethod.Get, uri);
        string accessToken = Preferences.Get("access_token", string.Empty);
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        try
        {
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshToken();
                accessToken = Preferences.Get("access_token", string.Empty);
                HttpRequestMessage newRequest = new(HttpMethod.Get, uri);
                newRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                response = await _client.SendAsync(newRequest);
            }
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<TracksResult>(content, _serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"\tERROR {0}", ex.Message);
        }
        return result?.Items ?? [];
    }
    public async Task<List<Artist>> GetFollowedArtists()
    {
        FollowedArtistsResult? result = new();
        List<Artist> followedArtists = [];
        Uri uri = new($"https://api.spotify.com/v1/me/following?type=artist&limit=50");
        HttpRequestMessage request = new(HttpMethod.Get, uri);
        string accessToken = Preferences.Get("access_token", string.Empty);
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        try
        {
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshToken();
                accessToken = Preferences.Get("access_token", string.Empty);
                HttpRequestMessage newRequest = new(HttpMethod.Get, uri);
                newRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                response = await _client.SendAsync(newRequest);
            }
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                JsonDocument jsonDocument = JsonDocument.Parse(content);
                JsonElement root = jsonDocument.RootElement;
                if (root.TryGetProperty("artists", out JsonElement artists))
                {
                    result = JsonSerializer.Deserialize<FollowedArtistsResult>(artists.GetRawText(), _serializerOptions) ?? new();
                    followedArtists = new List<Artist>(result.Items);
                }
                while(result.Next != null)
                {
                    uri = new(result.Next);
                    request = new(HttpMethod.Get, uri);
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    response = await _client.SendAsync(request);
                    content = await response.Content.ReadAsStringAsync();
                    jsonDocument = JsonDocument.Parse(content);
                    root = jsonDocument.RootElement;
                    if (root.TryGetProperty("artists", out JsonElement moreArtists))
                    {
                        result = JsonSerializer.Deserialize<FollowedArtistsResult>(moreArtists.GetRawText(), _serializerOptions) ?? new();
                        foreach (Artist artist in result.Items)
                        {
                            followedArtists.Add(artist);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"\tERROR {0}", ex.Message);
        }
        return followedArtists;
    }
    public async Task<bool[]> CheckArtistsFollowed(string[] query)
    {
        bool[]? followed = new bool[query.Length];
        string artistIds = string.Join(",", query);
        Uri uri = new($"https://api.spotify.com/v1/me/following/contains?type=artist&ids={artistIds}");
        HttpRequestMessage request = new(HttpMethod.Get, uri);
        string accessToken = Preferences.Get("access_token", string.Empty);
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        try
        {
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshToken();
                accessToken = Preferences.Get("access_token", string.Empty);
                HttpRequestMessage newRequest = new(HttpMethod.Get, uri);
                newRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                response = await _client.SendAsync(newRequest);
            }
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                followed = JsonSerializer.Deserialize<bool[]>(content, _serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"\tERROR {0}", ex.Message);
        }
        return followed ?? new bool[query.Length];
    }
    public async Task<List<SavedTrack>> GetSavedTracks()
    {
        SavedTracksResult? result = new();
        List<SavedTrack>? savedTracks = [];
        Uri uri = new($"https://api.spotify.com/v1/me/tracks?limit=50");
        HttpRequestMessage request = new(HttpMethod.Get, uri);
        string accessToken = Preferences.Get("access_token", string.Empty);
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        try
        {
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshToken();
                accessToken = Preferences.Get("access_token", string.Empty);
                HttpRequestMessage newRequest = new(HttpMethod.Get, uri);
                newRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                response = await _client.SendAsync(newRequest);
            }
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<SavedTracksResult>(content, _serializerOptions) ?? new();
                savedTracks = new List<SavedTrack>(result.Items);
                while (result.Next != null)
                {
                    uri = new(result.Next);
                    request = new(HttpMethod.Get, uri);
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    response = await _client.SendAsync(request);
                    content = await response.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<SavedTracksResult>(content, _serializerOptions) ?? new();
                    foreach (SavedTrack track in result.Items)
                    {
                        savedTracks.Add(track);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"\tERROR {0}", ex.Message);
        }
        return savedTracks ?? [];
    }
    public async Task<bool[]> CheckTracksSaved(string[] query)
    {
        bool[]? saved = new bool[query.Length];
        string trackIds = string.Join(",", query);
        Uri uri = new($"https://api.spotify.com/v1/me//tracks/contains?ids={trackIds}");
        HttpRequestMessage request = new(HttpMethod.Get, uri);
        string accessToken = Preferences.Get("access_token", string.Empty);
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        try
        {
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshToken();
                accessToken = Preferences.Get("access_token", string.Empty);
                HttpRequestMessage newRequest = new(HttpMethod.Get, uri);
                newRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                response = await _client.SendAsync(newRequest);
            }
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                saved = JsonSerializer.Deserialize<bool[]>(content, _serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"\tERROR {0}", ex.Message);
        }
        return saved ?? new bool[query.Length];
    }
    public async Task<List<Track>> GetRecentlyPlayedTracks(int limit)
    {
        throw new NotImplementedException();
    }

    private static readonly Func<int, string> generateRandomString = length =>
    {
        const string possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        return new string(bytes.Select(b => possible[b % possible.Length]).ToArray());
    };
    private static readonly Func<string, Task<byte[]>> sha256 = async input =>
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        return await Task.Run(() => SHA256.HashData(bytes));
    };
    private static readonly Func<byte[], string> base64encode = input =>
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    };
    private static async Task<string?> GetAuthorizationCode()
    {
        string codeVerifier = generateRandomString(64);
        byte[] hashed = await sha256(codeVerifier);
        string codeChallenge = base64encode(hashed);
        string scope = "user-read-private user-read-email user-top-read user-follow-read user-library-read user-read-recently-played";

        Preferences.Set("code_verifier", codeVerifier);

        var queryParams = new Dictionary<string, string?>
    {
        { "response_type", "code" },
        { "client_id", clientId },
        { "scope", scope },
        { "redirect_uri", redirectUri },
        { "code_challenge_method", "S256" },
        { "code_challenge", codeChallenge }
    };
        string authUrl = "https://accounts.spotify.com/authorize?" + string.Join("&",
        queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}"));

        try
        {
#if !WINDOWS
            WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(new Uri(authUrl), new Uri(redirectUri));
            Uri callbackUri = new Uri(authResult.CallbackUri.AbsoluteUri);
            var queryParamsResult = HttpUtility.ParseQueryString(callbackUri.Query);
            string? code = queryParamsResult["code"];
            if (code != null)
            {
                return queryParamsResult["code"]; // Return the authorization code
            }
#else
            throw new PlatformNotSupportedException("WebAuthenticator is not supported on Windows.");
#endif
        }
        catch (Exception ex) // TaskCanceledException 
        {
            Console.WriteLine($"Authentication failed: {ex.Message}");
        }
        return null;
    }
    private async Task<bool> RefreshToken()
    {
        Token? Token;
        Uri uri = new($"https://accounts.spotify.com/api/token");;
        string refreshToken = Preferences.Get("refresh_token", string.Empty);
        var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", clientId)
            ]);
        HttpRequestMessage request = new(HttpMethod.Post, uri)
        {
            Content = content
        };
        try
        {
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Token = JsonSerializer.Deserialize<Token>(responseContent, _serializerOptions);
                if (Token != null)
                {
                    Preferences.Set("access_token", Token.Access_token);
                    Preferences.Set("refresh_token", Token.Refresh_token);
                }
                return true;
            }
            else
            {
                Debug.WriteLine("Failed to get access token.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            return false;
        }

    }
}
