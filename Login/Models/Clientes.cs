using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("Clientes", Schema = "dbo")]
public class Clientes
{
    [Key]
    public int IdCliente { get; set; }
    public int? CedulaCliente { get; set; } = 0;
    public string? NombreCliente { get; set; }
    public string? EmpresaCliente { get; set; }
    public string? CiudadCliente { get; set; }
    public string? TelefonoCliente { get; set; }
    public string? CorreoCliente { get; set; }
    public string? DireccionCliente { get; set; }
}
