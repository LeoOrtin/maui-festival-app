using FestivalPlannerApp.Models;

namespace FestivalPlannerApp.Services
{
    public interface ISpotifyService
    {
        Task<bool> Authenticate();
        bool IsSignedIn();
        Task SignOut();
        Task<List<Artist>> SearchArtists(string query);
        Task<List<Artist>> GetTopArtists();
    }
}
