using Dominio.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Dominio.Interfaces;

namespace Infraestructura.Repositorios
{
    public class RepositorioPersonalADO : IRepositorioPersonal
    {
        private readonly string _connectionString;

        public RepositorioPersonalADO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Enfermera? ObtenerEnfermeraPorUsuario(int idUsuario)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                var query = SqlQueries.Personal.ObtenerEnfermeraPorUsuario;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Enfermera
                            {
                                Nombre = reader["Nombre"].ToString(),
                                Apellido = reader["Apellido"].ToString(),
                                DNI = Convert.ToInt32(reader["DNI"]),
                                Matricula = reader["Matricula"].ToString(),
                                CUIL = reader["CUIL"]?.ToString() ?? string.Empty,
                                Email = reader["Email"]?.ToString() ?? string.Empty,
                                Telefono = reader["Telefono"] != DBNull.Value ? Convert.ToInt64(reader["Telefono"]) : 0,
                                FechaNacimiento = reader["FechaNacimiento"] != DBNull.Value ? Convert.ToDateTime(reader["FechaNacimiento"]) : DateTime.MinValue
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Doctor? ObtenerDoctorPorUsuario(int idUsuario)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                var query = SqlQueries.Personal.ObtenerDoctorPorUsuario;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Doctor
                            {
                                Nombre = reader["Nombre"].ToString(),
                                Apellido = reader["Apellido"].ToString(),
                                DNI = Convert.ToInt32(reader["DNI"]),
                                Matricula = reader["Matricula"].ToString(),
                                CUIL = reader["CUIL"]?.ToString() ?? string.Empty,
                                Email = reader["Email"]?.ToString() ?? string.Empty,
                                Telefono = reader["Telefono"] != DBNull.Value ? Convert.ToInt64(reader["Telefono"]) : 0,
                                FechaNacimiento = reader["FechaNacimiento"] != DBNull.Value ? Convert.ToDateTime(reader["FechaNacimiento"]) : DateTime.MinValue
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void GuardarEnfermera(Enfermera enfermera, int idUsuario)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                var query = SqlQueries.Personal.InsertarEnfermera;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@Nombre", enfermera.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", enfermera.Apellido);
                    cmd.Parameters.AddWithValue("@DNI", enfermera.DNI);
                    cmd.Parameters.AddWithValue("@CUIL", enfermera.CUIL);
                    cmd.Parameters.AddWithValue("@Matricula", enfermera.Matricula);
                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(enfermera.Email) ? DBNull.Value : enfermera.Email);
                    cmd.Parameters.AddWithValue("@Telefono", enfermera.Telefono == 0 ? DBNull.Value : enfermera.Telefono);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", enfermera.FechaNacimiento == DateTime.MinValue ? DBNull.Value : enfermera.FechaNacimiento);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void GuardarDoctor(Doctor doctor, int idUsuario)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                var query = SqlQueries.Personal.InsertarDoctor;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@Nombre", doctor.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", doctor.Apellido);
                    cmd.Parameters.AddWithValue("@DNI", doctor.DNI);
                    cmd.Parameters.AddWithValue("@CUIL", doctor.CUIL);
                    cmd.Parameters.AddWithValue("@Matricula", doctor.Matricula);
                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(doctor.Email) ? DBNull.Value : doctor.Email);
                    cmd.Parameters.AddWithValue("@Telefono", doctor.Telefono == 0 ? DBNull.Value : doctor.Telefono);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", doctor.FechaNacimiento == DateTime.MinValue ? DBNull.Value : doctor.FechaNacimiento);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}