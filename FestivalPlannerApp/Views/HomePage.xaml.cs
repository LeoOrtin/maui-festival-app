using FestivalPlannerApp.ViewModels;

namespace FestivalPlannerApp.Views;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}