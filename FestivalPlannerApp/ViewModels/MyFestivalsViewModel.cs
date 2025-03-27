using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Models;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.Views;
using System.Collections.ObjectModel;

namespace FestivalPlannerApp.ViewModels;

public partial class MyFestivalsViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IAlertService _alertService;
    private readonly ISpotifyService _spotifyService;
    private User user = new();
    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<Festival>? MyFestivals {  get; set; }
    public MyFestivalsViewModel(IDatabaseService databaseService, IAlertService alertService, ISpotifyService spotifyService)
    {
        _databaseService = databaseService;
        _alertService = alertService;
        _spotifyService = spotifyService;
        Task.Run(async () => user = await spotifyService.GetUser()).Wait();
    }
    [RelayCommand]
    public async Task NavigatedTo()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            MyFestivals = new ObservableCollection<Festival>(await _databaseService.GetFestivalsAsync(user.Id));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]
    public async Task Refresh()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            MyFestivals = new ObservableCollection<Festival>(await _databaseService.GetFestivalsAsync(user.Id));
            IsRefreshing = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]
    public async Task NewFestival()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"/{nameof(NewFestivalPage)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]
    async Task ViewFestival(Festival festival)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"/{nameof(FestivalInfoPage)}", true,
            new Dictionary<string, object>
            {
            { "FestivalId", festival.Id }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]
    async Task EditFestival(Festival festival)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"/{nameof(EditFestivalPage)}", true,
                new Dictionary<string, object>
                {
                    { "FestivalId", festival.Id }
                });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]
    async Task DeleteFestival(Festival festival)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            if (await _alertService.ShowConfirmation($"Deleting Festival{festival.Name}", "Are you sure you want to delete this festival?"))
            {
                await _databaseService.DeleteFestivalAsync(festival);
                MyFestivals = new ObservableCollection<Festival>(await _databaseService.GetFestivalsAsync(user.Id));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
