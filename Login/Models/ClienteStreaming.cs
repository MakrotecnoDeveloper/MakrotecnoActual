using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models
{
    [Table("ClienteStreaming")]
    public class ClienteStreaming
    {
        [Key]
        public int IdClienteStreaming { get; set; }

        public string? NombreCliente { get; set; }

        public string? CelularCliente { get; set; }

        public string? Correo { get; set; }

        public int Estado { get; set; } = 1;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<ClientesPlataforma>? PlataformasCliente { get; set; }
    }
}
