using System.Windows.Input;
using Microsoft.Maui.Controls;
using TrashTracker.Services;

namespace TrashTracker.ViewModels
{
    public class InformacionUsuarioViewModel
    {
        private readonly ApiService _apiService = new();

        public string Nombre { get; set; } = Sesion.Actual?.Nombre ?? "";
        public string Apellido { get; set; } = Sesion.Actual?.Apellido ?? "";
        public string Correo { get; } = Sesion.Actual?.Correo ?? "";
        public string Rol { get; } = Sesion.Actual?.Rol ?? "";

        public ICommand GuardarCommand { get; }

        public InformacionUsuarioViewModel()
        {
            GuardarCommand = new Command(async () => await GuardarAsync());
        }

        private async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido))
            {
                await Shell.Current.DisplayAlert("Error", "El nombre y el apellido no pueden estar vacíos.", "OK");
                return;
            }

            bool ok = await _apiService.ActualizarUsuarioAsync(Nombre, Apellido);
            await Shell.Current.DisplayAlert(
                ok ? "Éxito" : "Error",
                ok ? "Datos actualizados correctamente." : "No se pudieron actualizar los datos.",
                "OK");
        }
    }
}