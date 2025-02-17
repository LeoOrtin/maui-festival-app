using FestivalPlannerApp.ViewModels;

namespace FestivalPlannerApp.Views;

public partial class MyFestivalsPage : ContentPage
{
	public MyFestivalsPage(MyFestivalsViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}