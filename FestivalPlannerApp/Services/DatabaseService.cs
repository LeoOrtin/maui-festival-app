using FestivalPlannerApp.Models;
using SQLite;

namespace FestivalPlannerApp.Services;

public class DatabaseService : IDatabaseService
{
    private readonly SQLiteAsyncConnection connection;
    public DatabaseService()
    {
        if (connection != null)
            return;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "mydatabase.db3");

        connection = new SQLiteAsyncConnection(dbPath);

        Task.Run(async () => await InitializeDatabase()).Wait();
    }
    private async Task InitializeDatabase()
    {
        try
        {
            await connection.CreateTableAsync<Festival>();
            await connection.CreateTableAsync<Concert>();
            await connection.CreateTableAsync<Stage>();
            await connection.CreateTableAsync<Day>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing database: {ex.Message}");
        }
    }
    public async Task AddFestivalAsync(Festival festival)
    {
        await connection.InsertAsync(festival);
    }
    public async Task<Festival> GetFestivalByIdAsync(int festivalId)
    {
        return await connection.Table<Festival>().Where(i => i.Id == festivalId).FirstOrDefaultAsync();
    }
    public async Task<List<Festival>> GetFestivalsAsync(string userId)
    {
        return await connection.Table<Festival>().Where(i => i.UserId == userId).ToListAsync();
    }
    public async Task UpdateFestivalAsync(Festival festival)
    {
        await connection.UpdateAsync(festival);
    }
    public async Task DeleteFestivalAsync(Festival festival)
    {
        await connection.ExecuteAsync("DELETE FROM concerts WHERE FestivalId = ?", festival.Id);
        await connection.ExecuteAsync("DELETE FROM stages WHERE FestivalId = ?", festival.Id);
        await connection.ExecuteAsync("DELETE FROM days WHERE FestivalId = ?", festival.Id);
        await connection.DeleteAsync(festival);
    }
    public async Task AddStageAsync(Stage stage, int festivalId)
    {
        stage.FestivalId = festivalId;
        await connection.InsertAsync(stage);
    }
    public async Task<List<Stage>> GetStagesAsync(int festivalId)
    {
        return await connection.Table<Stage>().Where(i => i.FestivalId == festivalId).ToListAsync();
    }
    public async Task AddDayAsync(Day day, int festivalId)
    {
        day.FestivalID = festivalId;
        await connection.InsertAsync(day);
    }
    public async Task<List<Day>>GetDaysAsync(int festivalId)
    {
        return await connection.Table<Day>().Where(i => i.FestivalID == festivalId).ToListAsync();
    }
    public async Task<Day> GetDayByIdAsync(int dayId)
    {
        return await connection.Table<Day>().Where(i => i.Id == dayId).FirstOrDefaultAsync();
    }
    public async Task AddConcertAsync(Concert concert)
    {
        await connection.InsertAsync(concert);
    }
    public async Task<List<Concert>> GetConcertsAsync(int festivalId)
    {
        return await connection.Table<Concert>().Where(i => i.FestivalId== festivalId).ToListAsync();
    }
    public async Task<Concert> GetConcertByNameAsync(string name, int festivalId)
    {
        return await connection.Table<Concert>().Where(i => i.FestivalId == festivalId && i.ArtistName == name).FirstOrDefaultAsync();
    }
    public async Task UpdateConcertAsync(Concert concert)
    {
        await connection.UpdateAsync(concert);
    }
    public async Task DeleteConcertAsync(Concert concert)
    {
        await connection.DeleteAsync(concert);
    }
}