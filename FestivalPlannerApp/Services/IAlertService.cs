namespace FestivalPlannerApp.Services;

public interface IAlertService
{
    Task ShowAlert(string title, string message);
    Task<bool> ShowConfirmation(string title, string message);
}