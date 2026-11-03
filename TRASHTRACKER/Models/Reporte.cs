using System;

namespace TrashTracker.Models
{
    public class Reporte
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TipoResiduo { get; set; } = string.Empty; // Orgánico, Reciclable, Peligroso, Escombros
        public string Urgencia { get; set; } = "Media";    // Baja, Media, Alta
        public string Descripcion { get; set; } = string.Empty;
        public string FotoRuta { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Estado { get; set; } = "Recibido";
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string CorreoUsuario { get; set; } = string.Empty;
    }
}
