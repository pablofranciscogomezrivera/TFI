using Dominio.Entidades;
using Dominio.Enums;
using Dominio.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infraestructura.Repositorios
{
    public class RepositorioUsuarioADO : IRepositorioUsuario
    {
        private readonly string _connectionString;

        public RepositorioUsuarioADO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Usuario? BuscarPorEmail(string email)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                var query = SqlQueries.Usuarios.BuscarPorEmail;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Email = reader["Email"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                TipoAutoridad = (TipoAutoridad)Convert.ToInt32(reader["TipoAutoridad"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool ExisteUsuarioConEmail(string email)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                var query = SqlQueries.Usuarios.ExisteUsuarioConEmail;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        public Usuario GuardarUsuario(Usuario usuario)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                var query = SqlQueries.Usuarios.InsertarUsuario;

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
                    cmd.Parameters.AddWithValue("@TipoAutoridad", (int)usuario.TipoAutoridad);

                    // Obtenemos el ID generado y lo asignamos al objeto
                    usuario.Id = (int)cmd.ExecuteScalar();
                }
            }
            return usuario;
        }
    }
}