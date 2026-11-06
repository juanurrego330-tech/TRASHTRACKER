using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using TrashTracker.Services;

namespace TrashTracker.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private string _nombre = string.Empty;
        private string _apellido = string.Empty;
        private string _correo = string.Empty;
        private string _password = string.Empty;
        private readonly ApiService _apiService = new();

        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(); }
        }

        public string Apellido
        {
            get => _apellido;
            set { _apellido = value; OnPropertyChanged(); }
        }

        public string Correo
        {
            get => _correo;
            set { _correo = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new Command(async () => await OnRegisterAsync());
            GoToLoginCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        private async Task OnRegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido) ||
                string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                return;
            }

            if (!Correo.EndsWith("@pascualbravo.edu.co"))
            {
                await Shell.Current.DisplayAlert("Acceso denegado", "Debe utilizar su correo institucional Pascual Bravo.", "OK");
                return;
            }

            bool exito = await _apiService.RegistrarUsuarioAsync(Correo, Password);
            if (exito)
            {
                await Shell.Current.DisplayAlert("Éxito", "Cuenta registrada correctamente. Ahora puedes iniciar sesión.", "OK");
                await Shell.Current.GoToAsync(".."); // Regresar al Login
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo completar el registro.", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}