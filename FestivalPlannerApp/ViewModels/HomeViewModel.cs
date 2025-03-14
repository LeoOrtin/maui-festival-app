﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FestivalPlannerApp.Models;
using FestivalPlannerApp.Services;
using FestivalPlannerApp.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
namespace FestivalPlannerApp.ViewModels
{
    public partial class HomeViewModel() : BaseViewModel
    {
        [RelayCommand]
        public async Task AddFestival()
        {
            await Shell.Current.GoToAsync($"/{nameof(NewFestivalPage)}");
        }
    }
}
