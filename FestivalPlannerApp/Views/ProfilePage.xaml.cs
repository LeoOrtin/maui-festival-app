using FestivalPlannerApp.ViewModels;

namespace FestivalPlannerApp.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}