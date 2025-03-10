using FestivalPlannerApp.ViewModels;

namespace FestivalPlannerApp.Views;

public partial class EditConcertPage : ContentPage
{
	public EditConcertPage(EditConcertViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}