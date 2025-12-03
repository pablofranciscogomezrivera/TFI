using Dominio.Entidades;
using Dominio.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infraestructura
{
    public class RepositorioPacientesADO : IRepositorioPacientes
    {
        private readonly string _connectionString;

        public RepositorioPacientesADO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Paciente? BuscarPacientePorCuil(string cuil)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                var query = SqlQueries.Pacientes.BuscarPorCuil;

                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Cuil", cuil);

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearPaciente(reader);
                        }
                    }
                }
            }
            return null;
        }

        public void GuardarPaciente(Paciente paciente)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                conexion.Open();
                using (var transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        var queryDomicilio = SqlQueries.Pacientes.InsertarDomicilio;

                        int idDomicilio;
                        using (var cmdDom = new SqlCommand(queryDomicilio, conexion, transaccion))
                        {
                            cmdDom.Parameters.AddWithValue("@Calle", paciente.Domicilio.Calle);
                            cmdDom.Parameters.AddWithValue("@Numero", paciente.Domicilio.Numero);
                            cmdDom.Parameters.AddWithValue("@Localidad", paciente.Domicilio.Localidad);

                            cmdDom.Parameters.AddWithValue("@Ciudad", paciente.Domicilio.Ciudad ?? "Tucumán");
                            cmdDom.Parameters.AddWithValue("@Provincia", paciente.Domicilio.Provincia ?? "Tucumán");

                            idDomicilio = (int)cmdDom.ExecuteScalar();
                        }

                        var queryPaciente = SqlQueries.Pacientes.InsertarPaciente;

                        using (var cmdPac = new SqlCommand(queryPaciente, conexion, transaccion))
                        {
                            cmdPac.Parameters.AddWithValue("@CUIL", paciente.CUIL);
                            cmdPac.Parameters.AddWithValue("@DNI", paciente.DNI);
                            cmdPac.Parameters.AddWithValue("@Nombre", paciente.Nombre);
                            cmdPac.Parameters.AddWithValue("@Apellido", paciente.Apellido);
                            cmdPac.Parameters.AddWithValue("@Email", paciente.Email ?? (object)DBNull.Value);
                            cmdPac.Parameters.AddWithValue("@Telefono", paciente.Telefono);
                            cmdPac.Parameters.AddWithValue("@FechaNacimiento", paciente.FechaNacimiento);
                            cmdPac.Parameters.AddWithValue("@IdDomicilio", idDomicilio);

                            if (paciente.Afiliado != null && paciente.Afiliado.ObraSocial != null)
                            {
                                cmdPac.Parameters.AddWithValue("@IdObraSocial", paciente.Afiliado.ObraSocial.Id);
                                cmdPac.Parameters.AddWithValue("@NumeroAfiliado", paciente.Afiliado.NumeroAfiliado);
                            }
                            else
                            {
                                cmdPac.Parameters.AddWithValue("@IdObraSocial", DBNull.Value);
                                cmdPac.Parameters.AddWithValue("@NumeroAfiliado", DBNull.Value);
                            }

                            cmdPac.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public Paciente RegistrarPaciente(Paciente paciente)
        {
            GuardarPaciente(paciente);
            return paciente;
        }

        private Paciente MapearPaciente(SqlDataReader reader)
        {
            var paciente = new Paciente
            {
                CUIL = reader["CUIL"].ToString(),
                DNI = Convert.ToInt32(reader["DNI"]),
                Nombre = reader["Nombre"].ToString(),
                Apellido = reader["Apellido"].ToString(),
                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                Telefono = reader["Telefono"] != DBNull.Value ? Convert.ToInt64(reader["Telefono"]) : 0,
                FechaNacimiento = Convert.ToDateTime(reader["FechaNacimiento"]),
                Domicilio = new Domicilio
                {
                    Calle = reader["Calle"].ToString(),
                    Numero = Convert.ToInt32(reader["Numero"]),
                    Localidad = reader["Localidad"].ToString(),
                    Ciudad = reader["Ciudad"].ToString(),
                    Provincia = reader["Provincia"].ToString()
                }
            };

            if (reader["IdObraSocial"] != DBNull.Value)
            {
                paciente.Afiliado = new Afiliado
                {
                    NumeroAfiliado = reader["NumeroAfiliado"].ToString(),
                    ObraSocial = new ObraSocial
                    {
                        Id = Convert.ToInt32(reader["IdObraSocial"]),
                        Nombre = reader["NombreObraSocial"].ToString()
                    }
                };
            }

            return paciente;
        }
    }
}