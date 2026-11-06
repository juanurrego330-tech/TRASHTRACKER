using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using SQLite;
using TrashTracker.Models;

namespace TrashTracker.Services
{
    public class ApiService
    {
        public async Task<bool> RegistrarUsuarioAsync(string nombre, string apellido, string correo, string password)
        {
            correo = correo.Trim().ToLowerInvariant();
            if (!correo.EndsWith("@pascualbravo.edu.co")) return false;

            var db = await BaseDatos.ObtenerAsync();
            try
            {
                int filas = await db.ExecuteAsync(
                    @"INSERT INTO Usuarios (Id_rol, Nombre_usuario, Apellido_usuario, Correo_usuario, Contrasena_usuario, fecha_creacion_usuario)
                      VALUES ((SELECT Id_rol FROM Roles WHERE Nombre_Rol = 'Estudiante'), ?, ?, ?, ?, ?)",
                    nombre.Trim(), apellido.Trim(), correo, Hash(correo, password), Ahora());
                return filas == 1;
            }
            catch (SQLiteException)
            {
                return false; // correo repetido (UNIQUE) u otro error
            }
        }

        public async Task<bool> LoginAsync(string correo, string password)
        {
            correo = correo.Trim().ToLowerInvariant();
            var db = await BaseDatos.ObtenerAsync();

            var usuarios = await db.QueryAsync<Usuario>(
                @"SELECT u.Id_usuario AS Id, u.Nombre_usuario AS Nombre, u.Apellido_usuario AS Apellido,
                         u.Correo_usuario AS Correo, r.Nombre_Rol AS Rol
                  FROM Usuarios u
                  JOIN Roles r ON r.Id_rol = u.Id_rol
                  WHERE u.Correo_usuario = ? AND u.Contrasena_usuario = ?",
                correo, Hash(correo, password));

            Sesion.Actual = usuarios.FirstOrDefault();
            return Sesion.Actual != null;
        }

        public async Task<bool> CrearReporteAsync(Reporte reporte)
        {
            if (Sesion.Actual == null) return false;

            var db = await BaseDatos.ObtenerAsync();
            try
            {
                int filas = await db.ExecuteAsync(
                    @"INSERT INTO Reportes (Id_usuario, Id_ubicacion, Id_estado_actual, Id_tipo_residuo, Id_urgencia,
                                            descripcion_reporte, nombre_foto, latitud, longitud, fecha_reporte)
                      VALUES (?,
                              (SELECT Id_ubicacion FROM Ubicaciones WHERE bloque_ubicacion = ? AND piso_ubicacion = ?),
                              (SELECT Id_Estado_Reporte FROM Estados_Reportes WHERE nombre_estado_reporte = 'Recibido'),
                              (SELECT Id_tipo_residuo FROM Tipos_residuos WHERE nombre_tipo_residuo = ?),
                              (SELECT Id_urgencia FROM Niveles_Urgencia WHERE nombre_urgencia = ?),
                              ?, ?, ?, ?, ?)",
                    Sesion.Actual.Id, reporte.Bloque, reporte.Piso, reporte.TipoResiduo, reporte.Urgencia,
                    reporte.Descripcion, reporte.NombreFoto, reporte.Latitud, reporte.Longitud, Ahora());
                return filas == 1;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }

        public async Task<List<Reporte>> ObtenerReportesAsync()
        {
            var db = await BaseDatos.ObtenerAsync();
            return await db.QueryAsync<Reporte>(
                @"SELECT r.Id_reporte AS Id,
                         t.nombre_tipo_residuo AS TipoResiduo,
                         n.nombre_urgencia AS Urgencia,
                         e.nombre_estado_reporte AS Estado,
                         ub.bloque_ubicacion AS Bloque,
                         ub.piso_ubicacion AS Piso,
                         r.descripcion_reporte AS Descripcion,
                         r.nombre_foto AS NombreFoto,
                         r.latitud AS Latitud,
                         r.longitud AS Longitud,
                         strftime('%d/%m/%Y %H:%M', r.fecha_reporte) AS Fecha,
                         u.Nombre_usuario || ' ' || u.Apellido_usuario AS Reportante
                  FROM Reportes r
                  JOIN Usuarios u ON u.Id_usuario = r.Id_usuario
                  JOIN Ubicaciones ub ON ub.Id_ubicacion = r.Id_ubicacion
                  JOIN Estados_Reportes e ON e.Id_Estado_Reporte = r.Id_estado_actual
                  JOIN Tipos_residuos t ON t.Id_tipo_residuo = r.Id_tipo_residuo
                  JOIN Niveles_Urgencia n ON n.Id_urgencia = r.Id_urgencia
                  ORDER BY r.fecha_reporte DESC");
        }

        public async Task<bool> ActualizarUsuarioAsync(string nombre, string apellido)
        {
            if (Sesion.Actual == null) return false;

            var db = await BaseDatos.ObtenerAsync();
            int filas = await db.ExecuteAsync(
                "UPDATE Usuarios SET Nombre_usuario = ?, Apellido_usuario = ? WHERE Id_usuario = ?",
                nombre.Trim(), apellido.Trim(), Sesion.Actual.Id);

            if (filas == 1)
            {
                Sesion.Actual.Nombre = nombre.Trim();
                Sesion.Actual.Apellido = apellido.Trim();
            }
            return filas == 1;
        }

        public async Task<bool> ExisteCorreoAsync(string correo)
        {
            var db = await BaseDatos.ObtenerAsync();
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Usuarios WHERE Correo_usuario = ?",
                correo.Trim().ToLowerInvariant()) > 0;
        }

        public async Task<bool> CambiarPasswordAsync(string correo, string nuevaPassword)
        {
            correo = correo.Trim().ToLowerInvariant();
            var db = await BaseDatos.ObtenerAsync();
            int filas = await db.ExecuteAsync(
                "UPDATE Usuarios SET Contrasena_usuario = ? WHERE Correo_usuario = ?",
                Hash(correo, nuevaPassword), correo);
            return filas == 1;
        }

        private static string Ahora() =>
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        // Se guarda el hash, nunca la contraseña real (el correo funciona como "sal")
        private static string Hash(string correo, string password) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(correo + ":" + password)));
    }
}