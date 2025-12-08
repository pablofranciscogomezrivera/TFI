using Dominio.Entidades;
using Dominio.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infraestructura.Repositorios
{
    public class RepositorioObraSocialADO : IRepositorioObraSocial
    {
        private readonly string _connectionString;

        public RepositorioObraSocialADO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void AgregarObraSocial(ObraSocial obraSocial)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                // Usamos MERGE o IF NOT EXISTS para evitar duplicados si el ID ya existe
                var query = SqlQueries.ObrasSociales.InsertarObraSocial;

                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", obraSocial.Id);
                    comando.Parameters.AddWithValue("@Nombre", obraSocial.Nombre);
                    comando.ExecuteNonQuery();
                }
            }
        }

        public ObraSocial? BuscarObraSocialPorId(int id)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                var query = SqlQueries.ObrasSociales.BuscarPorId;

                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ObraSocial
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool ExisteObraSocial(int id)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                var query = SqlQueries.ObrasSociales.ExisteObraSocial;

                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    return (int)comando.ExecuteScalar() > 0;
                }
            }
        }
        public List<ObraSocial> ObtenerTodas()
        {
            var lista = new List<ObraSocial>();
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                var query = SqlQueries.ObrasSociales.ObtenerTodas;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new ObraSocial
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }
        public bool EstaAfiliadoAObraSocial(int obraSocialId, string numeroAfiliado)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                // Validamos contra la tabla PadronAfiliados que creamos en el paso 1
                var query = SqlQueries.ObrasSociales.ValidarAfiliacion;

                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@IdObraSocial", obraSocialId);
                    comando.Parameters.AddWithValue("@NumeroAfiliado", numeroAfiliado);

                    return (int)comando.ExecuteScalar() > 0;
                }
            }
        }
    }
}