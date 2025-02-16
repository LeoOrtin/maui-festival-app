namespace FestivalPlannerApp.Services
{
    public class AlertService : IAlertService
    {
        public async Task ShowAlert(string title, string message)
        {
            await Shell.Current.DisplayAlert(title, message, "OK");
        }
        public async Task<bool> ShowConfirmation(string title, string message)
        {
            return await Shell.Current.DisplayAlert(title, message, "Yes", "No");
        }
    }
}
