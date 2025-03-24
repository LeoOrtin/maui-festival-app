using FestivalPlannerApp.Services;
using FestivalPlannerApp.ViewModels;

namespace FestivalPlannerApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}