using FestivalPlannerApp.Views;

namespace FestivalPlannerApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));

            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));

            Routing.RegisterRoute(nameof(NewFestivalPage), typeof(NewFestivalPage));

            Routing.RegisterRoute(nameof(EditFestivalPage), typeof(EditFestivalPage));

            Routing.RegisterRoute(nameof(FestivalInfoPage), typeof(FestivalInfoPage));

            Routing.RegisterRoute(nameof(MyFestivalsPage), typeof(MyFestivalsPage));

            Routing.RegisterRoute(nameof(EditConcertPage), typeof(EditConcertPage));
        }
    }
}
