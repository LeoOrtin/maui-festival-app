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

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "database.db3");

        connection = new SQLiteAsyncConnection(dbPath);

        connection.CreateTableAsync<Festival>();
        connection.CreateTableAsync<Concert>();
    }

    public async Task AddConcert(Concert concert)
    {
        await connection.InsertAsync(concert);
    }

    public async Task AddFestival(Festival festival)
    {
        await connection.InsertAsync(festival);
    }

    public async Task DeleteConcert(Concert concert)
    {
        await connection.DeleteAsync(concert);
    }

    public async Task DeleteFestival(Festival festival)
    {
        await connection.DeleteAsync(festival);
    }

    public async Task<List<Concert>> GetConcerts(int festivalId)
    {
        return await connection.Table<Concert>().Where(c => c.FestivalId == festivalId).ToListAsync();
    }

    public Task<List<Festival>> GetFestivals()
    {
        return connection.Table<Festival>().ToListAsync();
    }

    public async Task UpdateConcert(Concert concert)
    {
        await connection.UpdateAsync(concert);
    }

    public async Task UpdateFestival(Festival festival)
    {
        await connection.UpdateAsync(festival);
    }
}
