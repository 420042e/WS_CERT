using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_CERT.Model
{
    public class Servidor
    {
        // [Key] marca esta propiedad como la clave primaria.
        [Key]
        public int Id { get; set; }

        public string? Nombre { get; set; } // string? permite que el valor sea nulo

        public string? Estado { get; set; }

        public string? IPv4 { get; set; }

        // [Column] es crucial cuando el nombre de la columna en la BD
        // no es un nombre válido para una propiedad en C# (contiene espacios).
        [Column("SISTEMA OPERATIVO")]
        public string? SistemaOperativo { get; set; }

        // [Required] indica que este campo no puede ser nulo.
        [Required]
        public string Sujeto { get; set; }

        [Required]
        public string Emisor { get; set; }

        [Required]
        public DateTime ValidoDesde { get; set; }

        [Required]
        public DateTime ValidoHasta { get; set; }

        // Esta fecha la pone la BD por defecto. No necesitamos marcarla como requerida
        // en el código de la aplicación al crear un nuevo registro.
        public DateTime FechaRegistro { get; set; }

        public string? Observacion { get; set; }
    }
}
