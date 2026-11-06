using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using TrashTracker.Services;
using TrashTracker.Views;

namespace TrashTracker.ViewModels
{
    public class MenuViewModel : INotifyPropertyChanged
    {
        public string Bienvenida =>
            $"Bienvenido, {Sesion.Actual?.Nombre} {Sesion.Actual?.Apellido}".TrimEnd();

        public ICommand GenerarReporteCommand { get; }
        public ICommand VerReportesCommand { get; }
        public ICommand InformacionUsuarioCommand { get; }

        public MenuViewModel()
        {
            GenerarReporteCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(CreateReportPage)));
            VerReportesCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(ReportesPage)));
            InformacionUsuarioCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(InformacionUsuarioPage)));
        }

        // Se llama al volver al menú (por si el usuario cambió su nombre)
        public void Refrescar() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bienvenida)));

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}