using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
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
        private string _descripcion = string.Empty;
        private readonly ApiService _apiService = new();

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
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                        using Stream sourceStream = await photo.OpenReadAsync();
                        using FileStream localFileStream = File.OpenWrite(localFilePath);
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

            double lat = 0, lon = 0;
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
                Descripcion = Descripcion,
                FotoRuta = PhotoPath,
                Latitud = lat,
                Longitud = lon
            };

            await _apiService.CrearReporteAsync(reporte);

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
