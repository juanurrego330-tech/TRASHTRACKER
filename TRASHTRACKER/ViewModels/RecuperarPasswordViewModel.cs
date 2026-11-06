using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using TrashTracker.Services;

namespace TrashTracker.ViewModels
{
    public class RecuperarPasswordViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService = new();
        private int _paso = 1;

        public string Correo { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string NuevaPassword { get; set; } = string.Empty;
        public string ConfirmarPassword { get; set; } = string.Empty;

        public int Paso
        {
            get => _paso;
            set
            {
                _paso = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EsPaso1));
                OnPropertyChanged(nameof(EsPaso2));
                OnPropertyChanged(nameof(EsPaso3));
            }
        }

        public bool EsPaso1 => Paso == 1;
        public bool EsPaso2 => Paso == 2;
        public bool EsPaso3 => Paso == 3;

        public ICommand EnviarCodigoCommand { get; }
        public ICommand VerificarCodigoCommand { get; }
        public ICommand CambiarPasswordCommand { get; }
        public ICommand VolverCommand { get; }

        public RecuperarPasswordViewModel()
        {
            EnviarCodigoCommand = new Command(async () => await EnviarCodigoAsync());
            VerificarCodigoCommand = new Command(async () => await VerificarCodigoAsync());
            CambiarPasswordCommand = new Command(async () => await CambiarPasswordAsync());
            VolverCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        private async Task EnviarCodigoAsync()
        {
            if (!await _apiService.ExisteCorreoAsync(Correo ?? ""))
            {
                await Shell.Current.DisplayAlert("Error", "No existe una cuenta con ese correo.", "OK");
                return;
            }

            Paso = 2;
            await Shell.Current.DisplayAlert("Código enviado",
                "Te enviamos un código a tu correo. (Por ahora funciona cualquier código de letras y números.)", "OK");
        }

        private async Task VerificarCodigoAsync()
        {
            if (!Regex.IsMatch(Codigo ?? "", "^[A-Za-z0-9]+$"))
            {
                await Shell.Current.DisplayAlert("Error", "El código solo puede contener letras y números.", "OK");
                return;
            }

            Paso = 3;
        }

        private async Task CambiarPasswordAsync()
        {
            if ((NuevaPassword ?? "").Length < 8)
            {
                await Shell.Current.DisplayAlert("Contraseña", "La contraseña debe tener mínimo 8 caracteres.", "OK");
                return;
            }

            if (NuevaPassword != ConfirmarPassword)
            {
                await Shell.Current.DisplayAlert("Contraseña", "Las contraseñas no coinciden.", "OK");
                return;
            }

            if (await _apiService.CambiarPasswordAsync(Correo, NuevaPassword))
            {
                await Shell.Current.DisplayAlert("Éxito", "Contraseña actualizada. Ya puedes iniciar sesión.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo cambiar la contraseña.", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}