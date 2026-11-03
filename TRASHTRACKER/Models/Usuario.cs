namespace TrashTracker.Models
{
    public class Usuario
    {
        public string Correo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty; // Estudiante, ServiciosGenerales, Administrador
    }
}
