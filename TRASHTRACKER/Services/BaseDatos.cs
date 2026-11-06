using SQLite;

namespace TrashTracker.Services
{
    public static class BaseDatos
    {
        public const int TotalBloques = 27;
        public const int MaxPisos = 7;

        private static SQLiteAsyncConnection? _db;
        private static readonly SemaphoreSlim _candado = new(1, 1);

        public static async Task<SQLiteAsyncConnection> ObtenerAsync()
        {
            if (_db != null) return _db;

            await _candado.WaitAsync();
            try
            {
                if (_db != null) return _db;

                var db = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "trashtracker.db3"));
                await db.ExecuteAsync("PRAGMA foreign_keys = ON");

                foreach (var sql in Tablas) await db.ExecuteAsync(sql);
                foreach (var sql in DatosIniciales) await db.ExecuteAsync(sql);
                await CargarUbicacionesAsync(db);

                _db = db;
                return db;
            }
            finally
            {
                _candado.Release();
            }
        }

        private static async Task CargarUbicacionesAsync(SQLiteAsyncConnection db)
        {
            if (await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Ubicaciones") > 0) return;

            await db.RunInTransactionAsync(conexion =>
            {
                for (int b = 1; b <= TotalBloques; b++)
                    for (int p = 1; p <= MaxPisos; p++)
                        conexion.Execute(
                            "INSERT INTO Ubicaciones (bloque_ubicacion, piso_ubicacion) VALUES (?, ?)",
                            b.ToString(), p.ToString());
            });
        }

        private static readonly string[] Tablas =
        {
            @"CREATE TABLE IF NOT EXISTS Roles (
                Id_rol INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre_Rol TEXT NOT NULL UNIQUE)",

            @"CREATE TABLE IF NOT EXISTS Usuarios (
                Id_usuario INTEGER PRIMARY KEY AUTOINCREMENT,
                Id_rol INTEGER NOT NULL,
                Nombre_usuario TEXT NOT NULL,
                Apellido_usuario TEXT NOT NULL,
                Correo_usuario TEXT NOT NULL UNIQUE,
                Contrasena_usuario TEXT NOT NULL,
                fecha_creacion_usuario TEXT NOT NULL,
                FOREIGN KEY (Id_rol) REFERENCES Roles(Id_rol))",

            @"CREATE TABLE IF NOT EXISTS Ubicaciones (
                Id_ubicacion INTEGER PRIMARY KEY AUTOINCREMENT,
                bloque_ubicacion TEXT NOT NULL,
                piso_ubicacion TEXT NOT NULL,
                UNIQUE (bloque_ubicacion, piso_ubicacion))",

            @"CREATE TABLE IF NOT EXISTS Estados_Reportes (
                Id_Estado_Reporte INTEGER PRIMARY KEY AUTOINCREMENT,
                nombre_estado_reporte TEXT NOT NULL UNIQUE)",

            @"CREATE TABLE IF NOT EXISTS Tipos_residuos (
                Id_tipo_residuo INTEGER PRIMARY KEY AUTOINCREMENT,
                nombre_tipo_residuo TEXT NOT NULL UNIQUE,
                descripcion_tipo_residuo TEXT)",

            @"CREATE TABLE IF NOT EXISTS Niveles_Urgencia (
                Id_urgencia INTEGER PRIMARY KEY AUTOINCREMENT,
                nombre_urgencia TEXT NOT NULL UNIQUE)",

            @"CREATE TABLE IF NOT EXISTS Reportes (
                Id_reporte INTEGER PRIMARY KEY AUTOINCREMENT,
                Id_usuario INTEGER NOT NULL,
                Id_ubicacion INTEGER NOT NULL,
                Id_estado_actual INTEGER NOT NULL,
                Id_tipo_residuo INTEGER NOT NULL,
                Id_urgencia INTEGER NOT NULL,
                descripcion_reporte TEXT,
                nombre_foto TEXT NOT NULL,
                latitud REAL,
                longitud REAL,
                fecha_reporte TEXT NOT NULL,
                FOREIGN KEY (Id_usuario) REFERENCES Usuarios(Id_usuario),
                FOREIGN KEY (Id_ubicacion) REFERENCES Ubicaciones(Id_ubicacion),
                FOREIGN KEY (Id_estado_actual) REFERENCES Estados_Reportes(Id_Estado_Reporte),
                FOREIGN KEY (Id_tipo_residuo) REFERENCES Tipos_residuos(Id_tipo_residuo),
                FOREIGN KEY (Id_urgencia) REFERENCES Niveles_Urgencia(Id_urgencia))"
        };

        private static readonly string[] DatosIniciales =
        {
            "INSERT OR IGNORE INTO Roles (Nombre_Rol) VALUES ('Estudiante'), ('ServiciosGenerales'), ('Administrador')",
            "INSERT OR IGNORE INTO Estados_Reportes (nombre_estado_reporte) VALUES ('Recibido'), ('En proceso'), ('Resuelto')",
            "INSERT OR IGNORE INTO Niveles_Urgencia (nombre_urgencia) VALUES ('Baja'), ('Media'), ('Alta')",
            "INSERT OR IGNORE INTO Tipos_residuos (nombre_tipo_residuo) VALUES ('Orgánico'), ('Reciclable'), ('Peligroso / Vidrio'), ('Escombros')"
        };
    }
}