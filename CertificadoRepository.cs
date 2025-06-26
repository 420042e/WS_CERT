// Asegúrate de tener estas directivas 'using' al principio del archivo.
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

// El namespace que tú definiste.
namespace WS_CERT
{
    /// <summary>
    /// Esta clase actúa como una Capa de Acceso a Datos (DAL).
    /// Su única responsabilidad es comunicarse con la tabla CertificadosServidor
    /// y traducir los datos hacia y desde el objeto CertificadoInfo.
    /// </summary>
    internal class CertificadoRepository
    {
        // Campo privado para almacenar la cadena de conexión de forma segura.
        private readonly string _connectionString;

        /// <summary>
        /// Constructor de la clase. Requiere la cadena de conexión para poder funcionar.
        /// </summary>
        /// <param name="connectionString">La cadena de conexión a la base de datos SQL Server.</param>
        public CertificadoRepository(string connectionString)
        {
            // Valida que la cadena de conexión no sea nula o vacía.
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "La cadena de conexión no puede ser nula o vacía.");
            }
            _connectionString = connectionString;
        }

        /// <summary>
        /// Guarda un nuevo registro de certificado en la base de datos.
        /// Realiza la conversión de los datos del objeto CertificadoInfo al formato de la tabla.
        /// </summary>
        /// <param name="certificado">Un objeto CertificadoInfo con la información a guardar.</param>
        public void Agregar(CertificadoInfo certificado)
        {
            // Consulta SQL parametrizada para insertar datos de forma segura.
            string sqlQuery = @"
                INSERT INTO dbo.CertificadosServidor 
                    (NombreServidor, Sujeto, Emisor, ValidoDesde, ValidoHasta, Observacion) 
                VALUES 
                    (@Host, @Sujeto, @Emisor, @ValidoDesde, @ValidoHasta, @Observacion);";

            // El bloque 'using' asegura que la conexión se cierre correctamente, incluso si hay un error.
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    // 1. Mapeo de propiedades a parámetros SQL.
                    command.Parameters.AddWithValue("@Host", certificado.Host);
                    command.Parameters.AddWithValue("@Sujeto", certificado.Sujeto);
                    command.Parameters.AddWithValue("@Emisor", certificado.Emisor);

                    // 2. TRADUCCIÓN: Convertir las fechas de 'string' a 'DateTime' para la base de datos.
                    // Esto es crucial para que la BD almacene las fechas correctamente.
                    if (DateTime.TryParse(certificado.ValidoDesde, out DateTime validoDesdeDate))
                    {
                        command.Parameters.AddWithValue("@ValidoDesde", validoDesdeDate);
                    }
                    if (DateTime.TryParse(certificado.ValidoHasta, out DateTime validoHastaDate))
                    {
                        command.Parameters.AddWithValue("@ValidoHasta", validoHastaDate);
                    }

                    // 3. Manejo de valores nulos para el campo Observacion.
                    // Si la observación es nula o vacía, se inserta un DBNull en la base de datos.
                    command.Parameters.AddWithValue("@Observacion",
                        string.IsNullOrEmpty(certificado.Observacion) ? (object)DBNull.Value : certificado.Observacion);

                    // Ejecuta la consulta de inserción.
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Obtiene una lista de certificados de la base de datos que coincidan con un host.
        /// Realiza la conversión de los datos de la tabla al formato del objeto CertificadoInfo.
        /// </summary>
        /// <param name="host">El nombre del host (servidor) a buscar.</param>
        /// <returns>Una lista de objetos CertificadoInfo.</returns>
        public List<CertificadoInfo> ObtenerPorHost(string host)
        {
            var certificados = new List<CertificadoInfo>();
            string sqlQuery = @"
                SELECT NombreServidor, Sujeto, Emisor, ValidoDesde, ValidoHasta, Observacion 
                FROM dbo.CertificadosServidor 
                WHERE NombreServidor = @Host 
                ORDER BY ValidoHasta DESC;";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Host", host);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var certificado = new CertificadoInfo
                            {
                                // 1. Mapeo de columnas a propiedades.
                                Host = (string)reader["NombreServidor"],
                                Sujeto = (string)reader["Sujeto"],
                                Emisor = (string)reader["Emisor"],

                                // 2. TRADUCCIÓN: Convertir las fechas de 'DateTime' a 'string' para el objeto.
                                // Usamos el formato "o" (ISO 8601) que es estándar y no ambiguo.
                                ValidoDesde = ((DateTime)reader["ValidoDesde"]).ToString("o"),
                                ValidoHasta = ((DateTime)reader["ValidoHasta"]).ToString("o"),

                                // 3. Manejo de valores nulos al leer desde la BD.
                                // Si el valor es DBNull, se asigna un string vacío a la propiedad.
                                Observacion = reader["Observacion"] == DBNull.Value ? string.Empty : (string)reader["Observacion"]
                            };
                            certificados.Add(certificado);
                        }
                    }
                }
            }
            return certificados;
        }
    }
}