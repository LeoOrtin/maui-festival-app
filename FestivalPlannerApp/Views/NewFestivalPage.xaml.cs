using FestivalPlannerApp.ViewModels;

namespace FestivalPlannerApp.Views;

public partial class NewFestivalPage : ContentPage
{
	public NewFestivalPage(NewFestivalViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        
    }
}