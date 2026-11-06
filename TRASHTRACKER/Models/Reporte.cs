namespace TrashTracker.Models
{
    public class Reporte
    {
        // Lo que llena el formulario
        public int Id { get; set; }
        public string TipoResiduo { get; set; } = string.Empty;
        public string Urgencia { get; set; } = "Media";
        public string Bloque { get; set; } = string.Empty;
        public string Piso { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string NombreFoto { get; set; } = string.Empty; // solo el nombre del archivo
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }

        // Solo lectura: vienen de la consulta de la lista
        public string Estado { get; set; } = "Recibido";
        public string Fecha { get; set; } = string.Empty;
        public string Reportante { get; set; } = string.Empty;

        // Calculadas (no son columnas)
        public string FotoRuta => Path.Combine(FileSystem.AppDataDirectory, NombreFoto);
        public string Ubicacion => $"Bloque {Bloque} - Piso {Piso}";
    }
}