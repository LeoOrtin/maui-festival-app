using FestivalPlannerApp.Models;

namespace FestivalPlannerApp.Services
{
    public interface IDatabaseService
    {
        Task AddFestivalAsync(Festival festival);
        Task<Festival> GetFestivalByIdAsync(int festivalId);
        Task<List<Festival>> GetFestivalsAsync();
        Task UpdateFestivalAsync(Festival festival);
        Task DeleteFestivalAsync(Festival festival);
        Task AddStageAsync(Stage stage, int festivalId);
        Task<List<Stage>> GetStagesAsync(int festivalId);
        Task AddDayAsync(Day day, int festivalID);
        Task<List<Day>> GetDaysAsync(int festivalID);
        Task<Day> GetDayByIdAsync(int dayId);
        Task AddConcertAsync(Concert concert);
        Task<List<Concert>> GetConcertsAsync(int festivalId);
        Task<Concert> GetConcertByNameAsync(string name, int festivalId);
        Task UpdateConcertAsync(Concert concert);
        Task DeleteConcertAsync(Concert concert);
    }
}
