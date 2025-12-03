using Dominio.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Dominio.Interfaces;

namespace Infraestructura
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
                                Matricula = reader["Matricula"].ToString()
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
                                Matricula = reader["Matricula"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}