using System.Windows.Input;
using Microsoft.Maui.Controls;
using TrashTracker.Views;

namespace TrashTracker.ViewModels
{
    public class MenuViewModel
    {
        public ICommand GenerarReporteCommand { get; }
        public ICommand InformacionUsuarioCommand { get; }

        public MenuViewModel()
        {
            GenerarReporteCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(CreateReportPage)));
            InformacionUsuarioCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(InformacionUsuarioPage)));
        }
    }
}