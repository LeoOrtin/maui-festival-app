using FestivalPlannerApp.ViewModels;

namespace FestivalPlannerApp.Views;

public partial class FestivalInfoPage : ContentPage
{
	public FestivalInfoPage(FestivalInfoViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
    protected override bool OnBackButtonPressed()
    {
        (BindingContext as FestivalInfoViewModel)?.BackButtonCommand.Execute(null);
        return true;
    }
}