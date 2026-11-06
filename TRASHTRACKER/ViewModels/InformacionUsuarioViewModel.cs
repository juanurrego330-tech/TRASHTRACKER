namespace TrashTracker.ViewModels
{
    public class InformacionUsuarioViewModel
    {
        // Datos supuestos por ahora.
        // Más adelante aquí se cargan los datos reales del usuario y se habilita la edición.
        public string Nombre { get; set; } = "Usuario";
        public string Apellido { get; set; } = "Usuario";
        public string Correo { get; set; } = "usuario@pascualbravo.edu.co";
        public string Rol { get; set; } = "Estudiante";
    }
}