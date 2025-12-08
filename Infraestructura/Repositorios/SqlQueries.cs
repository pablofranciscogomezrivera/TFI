namespace Infraestructura.Repositorios
{
    /// <summary>
    /// Clase que centraliza todas las consultas SQL del sistema
    /// Facilita el mantenimiento y evita duplicación de código
    /// </summary>
    public static class SqlQueries
    {
        #region Pacientes

        public static class Pacientes
        {
            public const string BuscarPorCuil = @"
                SELECT p.*, d.Calle, d.Numero, d.Localidad, d.Ciudad, d.Provincia, os.Nombre as NombreObraSocial
                FROM Pacientes p
                INNER JOIN Domicilios d ON p.IdDomicilio = d.Id
                LEFT JOIN ObrasSociales os ON p.IdObraSocial = os.Id
                WHERE p.CUIL = @Cuil";

            public const string InsertarDomicilio = @"
                INSERT INTO Domicilios (Calle, Numero, Localidad, Ciudad, Provincia)
                VALUES (@Calle, @Numero, @Localidad, @Ciudad, @Provincia);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            public const string InsertarPaciente = @"
                INSERT INTO Pacientes (CUIL, DNI, Nombre, Apellido, Email, Telefono, FechaNacimiento, IdDomicilio, IdObraSocial, NumeroAfiliado)
                VALUES (@CUIL, @DNI, @Nombre, @Apellido, @Email, @Telefono, @FechaNacimiento, @IdDomicilio, @IdObraSocial, @NumeroAfiliado)";
        }

        #endregion

        #region Urgencias

        public static class Urgencias
        {
            public const string InsertarIngreso = @"
                INSERT INTO Ingresos (
                    IdPaciente, MatriculaEnfermera, FechaIngreso, Informe, 
                    NivelEmergencia, Estado, Temperatura, 
                    FrecuenciaCardiaca, FrecuenciaRespiratoria, 
                    TensionSistolica, TensionDiastolica
                ) VALUES (
                    @IdPaciente, @MatriculaEnfermera, @FechaIngreso, @Informe, 
                    @NivelEmergencia, @Estado, @Temperatura, 
                    @FrecuenciaCardiaca, @FrecuenciaRespiratoria, 
                    @TensionSistolica, @TensionDiastolica
                )";

            public const string ObtenerIngresosPendientes = @"
                SELECT i.*, p.CUIL, p.Nombre, p.Apellido, p.DNI
                FROM Ingresos i
                INNER JOIN Pacientes p ON i.IdPaciente = p.Id
                WHERE i.Estado = 0 -- 0 = PENDIENTE
                ORDER BY i.NivelEmergencia ASC, i.FechaIngreso ASC";

            public const string ActualizarIngreso = @"
                UPDATE Ingresos 
                SET Estado = @Estado,
                    MatriculaDoctor = @MatriculaDoctor,
                    InformeMedico = @InformeMedico
                WHERE FechaIngreso = @FechaIngreso 
                AND IdPaciente = (SELECT Id FROM Pacientes WHERE CUIL = @Cuil)";

            public const string BuscarIngresoPorCuilYEstado = @"
                SELECT i.*, p.CUIL, p.Nombre, p.Apellido, p.DNI
                FROM Ingresos i
                INNER JOIN Pacientes p ON i.IdPaciente = p.Id
                WHERE p.CUIL = @Cuil AND i.Estado = @Estado";

            public const string ObtenerTodosLosIngresos = @"
                SELECT i.*, p.CUIL, p.Nombre, p.Apellido, p.DNI
                FROM Ingresos i
                INNER JOIN Pacientes p ON i.IdPaciente = p.Id
                ORDER BY i.NivelEmergencia ASC, i.FechaIngreso ASC";

            public const string ObtenerIdPacientePorCuil =
                "SELECT Id FROM Pacientes WHERE CUIL = @Cuil";
        }

        #endregion

        #region Usuarios

        public static class Usuarios
        {
            public const string BuscarPorEmail =
                "SELECT Id, Email, PasswordHash, TipoAutoridad FROM Usuarios WHERE Email = @Email";

            public const string ExisteUsuarioConEmail =
                "SELECT COUNT(1) FROM Usuarios WHERE Email = @Email";

            public const string InsertarUsuario = @"
                INSERT INTO Usuarios (Email, PasswordHash, TipoAutoridad)
                VALUES (@Email, @PasswordHash, @TipoAutoridad);
                SELECT CAST(SCOPE_IDENTITY() as int);";
        }

        #endregion

        #region Obras Sociales

        public static class ObrasSociales
        {
            public const string InsertarObraSocial = @"
                IF NOT EXISTS (SELECT 1 FROM ObrasSociales WHERE Id = @Id)
                BEGIN
                    SET IDENTITY_INSERT ObrasSociales ON;
                    INSERT INTO ObrasSociales (Id, Nombre) VALUES (@Id, @Nombre);
                    SET IDENTITY_INSERT ObrasSociales OFF;
                END";

            public const string BuscarPorId =
                "SELECT Id, Nombre FROM ObrasSociales WHERE Id = @Id";

            public const string ExisteObraSocial =
                "SELECT COUNT(1) FROM ObrasSociales WHERE Id = @Id";

            public const string ObtenerTodas =
                "SELECT Id, Nombre FROM ObrasSociales ORDER BY Nombre ASC";

            public const string ValidarAfiliacion =
                "SELECT COUNT(1) FROM PadronAfiliados WHERE IdObraSocial = @IdObraSocial AND NumeroAfiliado = @NumeroAfiliado";
        }

        #endregion

        #region Personal

        public static class Personal
        {
            public const string ObtenerEnfermeraPorUsuario =
                "SELECT * FROM Enfermeros WHERE IdUsuario = @IdUsuario";

            public const string ObtenerDoctorPorUsuario =
                "SELECT * FROM Doctores WHERE IdUsuario = @IdUsuario";

            public const string InsertarEnfermera = @"
                INSERT INTO Enfermeros (IdUsuario, Nombre, Apellido, DNI, CUIL, Matricula, Email, Telefono, FechaNacimiento)
                VALUES (@IdUsuario, @Nombre, @Apellido, @DNI, @CUIL, @Matricula, @Email, @Telefono, @FechaNacimiento)";

            public const string InsertarDoctor = @"
                INSERT INTO Doctores (IdUsuario, Nombre, Apellido, DNI, CUIL, Matricula, Email, Telefono, FechaNacimiento)
                VALUES (@IdUsuario, @Nombre, @Apellido, @DNI, @CUIL, @Matricula, @Email, @Telefono, @FechaNacimiento)";
        }

        #endregion
    }
}
