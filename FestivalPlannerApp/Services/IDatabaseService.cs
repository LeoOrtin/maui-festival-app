using FestivalPlannerApp.Models;

namespace FestivalPlannerApp.Services
{
    public interface IDatabaseService
    {
        Task AddFestival (Festival festival);
        Task<List<Festival>> GetFestivals();
        Task UpdateFestival(Festival festival);
        Task DeleteFestival(Festival festival);
        Task AddConcert(Concert concert);
        Task<List<Concert>> GetConcerts(int festivalId);
        Task UpdateConcert(Concert concert);
        Task DeleteConcert(Concert concert);
    }
}
