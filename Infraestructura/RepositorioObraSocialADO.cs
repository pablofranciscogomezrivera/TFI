using Dominio.Entidades;
using Dominio.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infraestructura
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
                var query = @"
                    IF NOT EXISTS (SELECT 1 FROM ObrasSociales WHERE Id = @Id)
                    BEGIN
                        SET IDENTITY_INSERT ObrasSociales ON;
                        INSERT INTO ObrasSociales (Id, Nombre) VALUES (@Id, @Nombre);
                        SET IDENTITY_INSERT ObrasSociales OFF;
                    END";

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
                var query = "SELECT Id, Nombre FROM ObrasSociales WHERE Id = @Id";

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
                var query = "SELECT COUNT(1) FROM ObrasSociales WHERE Id = @Id";

                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    return (int)comando.ExecuteScalar() > 0;
                }
            }
        }

        public bool EstaAfiliadoAObraSocial(int obraSocialId, string numeroAfiliado)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                // Validamos contra la tabla PadronAfiliados que creamos en el paso 1
                var query = "SELECT COUNT(1) FROM PadronAfiliados WHERE IdObraSocial = @IdObraSocial AND NumeroAfiliado = @NumeroAfiliado";

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