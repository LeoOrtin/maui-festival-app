namespace FestivalPlannerApp.Views;

public partial class LoadingPage : ContentPage
{
    public LoadingPage()
    {
        InitializeComponent();
    }
    override protected async void OnAppearing()
    {
        await Task.Delay(2000);
        //base.OnAppearing();
        //if (await database.GetLoggedInUser() != null)
        //{
        //    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        //}
        //else
        //{
        //    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        //}
    }
}