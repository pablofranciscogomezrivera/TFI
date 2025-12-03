using Dominio.Entidades;
using Dominio.Entidades.ValueObject;
using Dominio.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infraestructura
{
    public class RepositorioUrgenciasADO : IRepositorioUrgencias
    {
        private readonly string _connectionString;

        public RepositorioUrgenciasADO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void AgregarIngreso(Ingreso ingreso)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();

                // 1. Necesitamos el ID del paciente en la BD
                var idPaciente = ObtenerIdPacientePorCuil(ingreso.Paciente.CUIL, conexion);

                var query = SqlQueries.Urgencias.InsertarIngreso;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);
                    cmd.Parameters.AddWithValue("@MatriculaEnfermera", ingreso.Enfermera.Matricula);
                    cmd.Parameters.Add("@FechaIngreso", SqlDbType.DateTime2).Value = ingreso.FechaIngreso;
                    cmd.Parameters.AddWithValue("@Informe", ingreso.InformeIngreso);
                    cmd.Parameters.AddWithValue("@NivelEmergencia", (int)ingreso.NivelEmergencia);
                    cmd.Parameters.AddWithValue("@Estado", (int)ingreso.Estado);

                    // Extraemos los valores primitivos de tus Value Objects
                    cmd.Parameters.AddWithValue("@Temperatura", ingreso.Temperatura.Valor);
                    cmd.Parameters.AddWithValue("@FrecuenciaCardiaca", ingreso.FrecuenciaCardiaca.Valor);
                    cmd.Parameters.AddWithValue("@FrecuenciaRespiratoria", ingreso.FrecuenciaRespiratoria.Valor);
                    cmd.Parameters.AddWithValue("@TensionSistolica", ingreso.TensionArterial.FrecuenciaSistolica.Valor);
                    cmd.Parameters.AddWithValue("@TensionDiastolica", ingreso.TensionArterial.FrecuenciaDiastolica.Valor);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Ingreso> ObtenerIngresosPendientes()
        {
            var ingresos = new List<Ingreso>();

            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                // AQUÍ LA BASE DE DATOS HACE EL TRABAJO DE LA COLA DE PRIORIDAD
                // Ordenamos por Nivel (0 es Crítico) y luego por Fecha (FIFO)
                var query = SqlQueries.Urgencias.ObtenerIngresosPendientes;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ingresos.Add(MapearIngreso(reader));
                        }
                    }
                }
            }
            return ingresos;
        }

        public void ActualizarIngreso(Ingreso ingreso)
        {
            Console.WriteLine($"[RepositorioUrgenciasADO] ActualizarIngreso llamado para CUIL: {ingreso.Paciente.CUIL}, Estado: {ingreso.Estado}");

            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();

                var query = SqlQueries.Urgencias.ActualizarIngreso;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    // 1. Estado
                    cmd.Parameters.AddWithValue("@Estado", (int)ingreso.Estado);
                    Console.WriteLine($"[RepositorioUrgenciasADO] Estado a actualizar: {(int)ingreso.Estado} ({ingreso.Estado})");

                    // 2. Matricula Doctor (Manejo de Nulos)
                    if (ingreso.Atencion != null && ingreso.Atencion.Doctor != null)
                    {
                        cmd.Parameters.AddWithValue("@MatriculaDoctor", ingreso.Atencion.Doctor.Matricula);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MatriculaDoctor", DBNull.Value);
                    }

                    // 3. Informe Médico (Manejo de Nulos)
                    // Asumimos que guardamos el informe completo de la atención
                    object informeVal = ingreso.Atencion?.Informe ?? (object)DBNull.Value;
                    cmd.Parameters.AddWithValue("@InformeMedico", informeVal);

                    // 4. Fecha Ingreso (CRÍTICO: Usar DateTime2 para precisión exacta)
                    cmd.Parameters.Add("@FechaIngreso", SqlDbType.DateTime2).Value = ingreso.FechaIngreso;

                    // 5. CUIL (Necesario para la subquery que busca el IdPaciente)
                    cmd.Parameters.AddWithValue("@Cuil", ingreso.Paciente.CUIL);

                    // Ejecutar y validar que se haya tocado alguna fila
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    Console.WriteLine($"[RepositorioUrgenciasADO] Filas afectadas: {filasAfectadas}");

                    if (filasAfectadas == 0)
                    {
                        Console.WriteLine($"[RepositorioUrgenciasADO] ERROR: No se actualizó ninguna fila para CUIL {ingreso.Paciente.CUIL}");
                        throw new Exception("Error de Concurrencia: No se actualizó el ingreso. Verifica que la FechaIngreso en la BD coincida exactamente (nanosegundos) con la del objeto.");
                    }

                    Console.WriteLine($"[RepositorioUrgenciasADO] Ingreso actualizado exitosamente para CUIL: {ingreso.Paciente.CUIL}");
                }
            }
        }

        public void RemoverIngreso(Ingreso ingreso)
        {
            // En base de datos no borramos, solo actualizamos estado.
            // La lógica de negocio ya llama a "ActualizarIngreso" después de esto,
            // así que este método puede quedar vacío o usarse para logs.
        }

        public Ingreso? BuscarIngresoPorCuilYEstado(string cuil, EstadoIngreso estado)
        {
            if (string.IsNullOrWhiteSpace(cuil))
                throw new ArgumentException("El CUIL no puede estar vacío", nameof(cuil));

            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();

                var query = SqlQueries.Urgencias.BuscarIngresoPorCuilYEstado;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Cuil", cuil);
                    cmd.Parameters.AddWithValue("@Estado", (int)estado);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearIngreso(reader);
                        }
                    }
                }
            }

            return null;
        }

        public List<Ingreso> ObtenerTodosLosIngresos()
        {
            var ingresos = new List<Ingreso>();

            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();

                var query = SqlQueries.Urgencias.ObtenerTodosLosIngresos;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ingresos.Add(MapearIngreso(reader));
                        }
                    }
                }
            }

            return ingresos;
        }

        // --- Helpers ---

        private int ObtenerIdPacientePorCuil(string cuil, SqlConnection conexion)
        {
            var cmd = new SqlCommand(SqlQueries.Urgencias.ObtenerIdPacientePorCuil, conexion);
            cmd.Parameters.AddWithValue("@Cuil", cuil);
            var result = cmd.ExecuteScalar();
            if (result == null) throw new Exception($"Error de Integridad: Paciente {cuil} no encontrado.");
            return (int)result;
        }

        private Ingreso MapearIngreso(SqlDataReader reader)
        {
            // Reconstruimos el objeto Paciente (versión resumida para la lista)
            var paciente = new Paciente
            {
                CUIL = reader["CUIL"].ToString(),
                Nombre = reader["Nombre"].ToString(),
                Apellido = reader["Apellido"].ToString(),
                DNI = Convert.ToInt32(reader["DNI"]),
                // Nota: Podrías necesitar traer el domicilio si lo muestras en la lista
            };

            var enfermera = new Enfermera
            {
                Matricula = reader["MatriculaEnfermera"].ToString(),
                Nombre = "Enfermera",
                Apellido = "Guardia"
            };

            // Usamos el constructor de tu Dominio para asegurar que las reglas se cumplan
            var ingreso = new Ingreso(
                paciente,
                enfermera,
                reader["Informe"].ToString(),
                (NivelEmergencia)Convert.ToInt32(reader["NivelEmergencia"]),
                Convert.ToDouble(reader["Temperatura"]),
                Convert.ToDouble(reader["FrecuenciaCardiaca"]),
                Convert.ToDouble(reader["FrecuenciaRespiratoria"]),
                Convert.ToDouble(reader["TensionSistolica"]),
                Convert.ToDouble(reader["TensionDiastolica"])
            );

            // Completamos los datos que no están en el constructor
            ingreso.FechaIngreso = Convert.ToDateTime(reader["FechaIngreso"]);
            ingreso.Estado = (EstadoIngreso)Convert.ToInt32(reader["Estado"]);

            // Si existe atención médica registrada, la reconstruimos
            // IMPORTANTE: Este bloque solo funciona si las columnas existen en la BD
            // Si no has ejecutado la migración, simplemente se salta este paso
            try
            {
                // Intentar acceder a las columnas de atención médica
                int ordMatriculaDoctor = reader.GetOrdinal("MatriculaDoctor");
                int ordInformeMedico = reader.GetOrdinal("InformeMedico");

                // Si llegamos aquí, las columnas existen
                if (reader["MatriculaDoctor"] != DBNull.Value)
                {
                    var doctor = new Doctor
                    {
                        Matricula = reader["MatriculaDoctor"].ToString(),
                        Nombre = "Doctor",
                        Apellido = "Guardia"
                    };

                    string informeMedico = reader["InformeMedico"] != DBNull.Value
                        ? reader["InformeMedico"].ToString()
                        : string.Empty;

                    ingreso.Atencion = new Atencion
                    {
                        Doctor = doctor,
                        Informe = informeMedico
                    };
                }
            }
            catch (IndexOutOfRangeException)
            {
                // Las columnas MatriculaDoctor/InformeMedico no existen en la BD
                // Esto es normal si no has ejecutado la migración SQL
                // El ingreso simplemente no tendrá atención (Atencion = null)
            }

            return ingreso;
        }
    }
}