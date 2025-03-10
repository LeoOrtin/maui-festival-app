using CommunityToolkit.Mvvm.ComponentModel;

namespace FestivalPlannerApp.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        public partial bool IsBusy { get; set; }
        public bool IsNotBusy => !IsBusy;
    }
}
