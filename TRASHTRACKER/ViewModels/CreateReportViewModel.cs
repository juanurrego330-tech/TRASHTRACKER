using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using TrashTracker.Models;
using TrashTracker.Services;

namespace TrashTracker.ViewModels
{
    public class CreateReportViewModel : INotifyPropertyChanged
    {
        private string _photoPath = string.Empty;
        private string _selectedTipoResiduo = string.Empty;
        private string _selectedUrgencia = "Media";
        private string _selectedBloque = string.Empty;
        private string _selectedPiso = string.Empty;
        private string _descripcion = string.Empty;
        private readonly ApiService _apiService = new();

        public List<string> Bloques { get; } =
            Enumerable.Range(1, BaseDatos.TotalBloques).Select(n => n.ToString()).ToList();
        public List<string> Pisos { get; } =
            Enumerable.Range(1, BaseDatos.MaxPisos).Select(n => n.ToString()).ToList();

        public string PhotoPath
        {
            get => _photoPath;
            set { _photoPath = value; OnPropertyChanged(); }
        }

        public string SelectedTipoResiduo
        {
            get => _selectedTipoResiduo;
            set { _selectedTipoResiduo = value; OnPropertyChanged(); }
        }

        public string SelectedUrgencia
        {
            get => _selectedUrgencia;
            set { _selectedUrgencia = value; OnPropertyChanged(); }
        }

        public string SelectedBloque
        {
            get => _selectedBloque;
            set { _selectedBloque = value; OnPropertyChanged(); }
        }

        public string SelectedPiso
        {
            get => _selectedPiso;
            set { _selectedPiso = value; OnPropertyChanged(); }
        }

        public string Descripcion
        {
            get => _descripcion;
            set { _descripcion = value; OnPropertyChanged(); }
        }

        public ICommand CapturePhotoCommand { get; }
        public ICommand SubmitReportCommand { get; }

        public CreateReportViewModel()
        {
            CapturePhotoCommand = new Command(async () => await CapturePhotoAsync());
            SubmitReportCommand = new Command(async () => await SubmitReportAsync());
        }

        private async Task CapturePhotoAsync()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        // AppDataDirectory es permanente (Cache puede ser borrada por Android)
                        string nombreArchivo = $"{Guid.NewGuid():N}{Path.GetExtension(photo.FileName)}";
                        string localFilePath = Path.Combine(FileSystem.AppDataDirectory, nombreArchivo);

                        using Stream sourceStream = await photo.OpenReadAsync();
                        using FileStream localFileStream = File.Create(localFilePath);
                        await sourceStream.CopyToAsync(localFileStream);

                        PhotoPath = localFilePath;
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo capturar la foto: {ex.Message}", "OK");
            }
        }

        private async Task SubmitReportAsync()
        {
            if (string.IsNullOrEmpty(PhotoPath))
            {
                await Shell.Current.DisplayAlert("Validación", "La fotografía del residuo es obligatoria.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(SelectedTipoResiduo))
            {
                await Shell.Current.DisplayAlert("Validación", "Debe seleccionar un tipo de residuo.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(SelectedBloque) || string.IsNullOrEmpty(SelectedPiso))
            {
                await Shell.Current.DisplayAlert("Validación", "Debe seleccionar el bloque y el piso.", "OK");
                return;
            }

            double? lat = null, lon = null;
            try
            {
                var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5)));
                if (location != null)
                {
                    lat = location.Latitude;
                    lon = location.Longitude;
                }
            }
            catch { }

            var reporte = new Reporte
            {
                TipoResiduo = SelectedTipoResiduo,
                Urgencia = SelectedUrgencia,
                Bloque = SelectedBloque,
                Piso = SelectedPiso,
                Descripcion = Descripcion,
                NombreFoto = Path.GetFileName(PhotoPath),
                Latitud = lat,
                Longitud = lon
            };

            bool guardado = await _apiService.CrearReporteAsync(reporte);
            if (!guardado)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo guardar el reporte. Intenta de nuevo.", "OK");
                return;
            }

            await Shell.Current.DisplayAlert("Éxito", "Reporte enviado correctamente al sistema del campus.", "OK");

            PhotoPath = string.Empty;
            SelectedTipoResiduo = string.Empty;
            Descripcion = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}