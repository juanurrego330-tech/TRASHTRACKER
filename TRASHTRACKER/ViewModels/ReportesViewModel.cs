using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TrashTracker.Models;
using TrashTracker.Services;

namespace TrashTracker.ViewModels
{
    public class ReportesViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private readonly ApiService _apiService = new();

        public ObservableCollection<Reporte> Reportes { get; } = new();

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand CargarCommand { get; }

        public ReportesViewModel()
        {
            CargarCommand = new Command(async () => await CargarAsync());
        }

        public async Task CargarAsync()
        {
            IsBusy = true;
            try
            {
                var lista = await _apiService.ObtenerReportesAsync();
                Reportes.Clear();
                foreach (var r in lista) Reportes.Add(r);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}