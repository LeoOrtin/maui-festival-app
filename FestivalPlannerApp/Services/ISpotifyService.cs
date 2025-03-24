using FestivalPlannerApp.Models;

namespace FestivalPlannerApp.Services;

public interface ISpotifyService
{
    Task<bool> Authenticate();
    bool IsSignedIn();
    void SignOut();
    Task<User> GetUser();
    Task<List<Artist>> SearchArtists(string query);
    Task<List<Artist>> GetTopArtists(string frame, int limit);
    Task<List<Track>> GetTopTracks(int limit);
    Task<List<Artist>> GetFollowedArtists();
    Task<bool[]> CheckArtistsFollowed(string[] query);
    Task<List<SavedTrack>> GetSavedTracks();
    Task<bool[]> CheckTracksSaved(string[] query);
    Task<List<Track>> GetRecentlyPlayedTracks(int limit);
}
