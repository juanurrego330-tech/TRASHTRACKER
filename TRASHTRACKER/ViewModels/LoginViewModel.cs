using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using TrashTracker.Services;
using TrashTracker.Views;

namespace TrashTracker.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        private string _correo = string.Empty;
        private string _password = string.Empty;
        private readonly ApiService _apiService = new();

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

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await OnLoginAsync());
            NavigateToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(RegisterPage)));
        }

        private async Task OnLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                return;
            }

            if (!Correo.EndsWith("@pascualbravo.edu.co"))
            {
                await Shell.Current.DisplayAlert("Acceso denegado", "Debe utilizar su correo institucional Pascual Bravo.", "OK");
                return;
            }

            bool exito = await _apiService.LoginAsync(Correo, Password);
            if (exito)
            {
                await Shell.Current.GoToAsync($"///{nameof(CreateReportPage)}");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Credenciales inválidas.", "OK");
            } 
        }
        public ICommand NavigateToRegisterCommand { get; }
    }
}
