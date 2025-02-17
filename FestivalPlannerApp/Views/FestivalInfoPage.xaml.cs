using FestivalPlannerApp.ViewModels;

namespace FestivalPlannerApp.Views;

public partial class FestivalInfoPage : ContentPage
{
	public FestivalInfoPage(FestivalInfoViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}