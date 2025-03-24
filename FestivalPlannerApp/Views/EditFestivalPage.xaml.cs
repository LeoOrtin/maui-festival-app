using FestivalPlannerApp.ViewModels;

namespace FestivalPlannerApp.Views;

public partial class EditFestivalPage : ContentPage
{
	public EditFestivalPage(EditFestivalViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
    protected override bool OnBackButtonPressed()
    {
        (BindingContext as EditFestivalViewModel)?.BackButtonCommand.Execute(null);
        return true;
    }
}