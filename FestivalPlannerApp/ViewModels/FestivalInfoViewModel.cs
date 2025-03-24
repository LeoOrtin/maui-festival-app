using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Views;

namespace FestivalPlannerApp.ViewModels;

public partial class FestivalInfoViewModel : BaseViewModel
{
    [RelayCommand]
    public async Task BackButton()
    {
        await Shell.Current.GoToAsync($"//{nameof(MyFestivalsPage)}");
    }
}
