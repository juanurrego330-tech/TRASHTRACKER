using System.Collections.Generic;
using System.Threading.Tasks;
using TrashTracker.Models;

namespace TrashTracker.Services
{
    public class ApiService
    {
        private static readonly List<Reporte> _reportesMock = new();

        public Task<bool> RegistrarUsuarioAsync(string correo, string password)
        {
            if (!correo.EndsWith("@pascualbravo.edu.co"))
                return Task.FromResult(false);
            
            return Task.FromResult(true);
        }

        public Task<bool> LoginAsync(string correo, string password)
        {
            bool isValid = correo.EndsWith("@pascualbravo.edu.co") && !string.IsNullOrEmpty(password);
            return Task.FromResult(isValid);
        }

        public Task<bool> CrearReporteAsync(Reporte reporte)
        {
            _reportesMock.Add(reporte);
            return Task.FromResult(true);
        }

        public Task<List<Reporte>> ObtenerReportesAsync()
        {
            return Task.FromResult(_reportesMock);
        }
    }
}
