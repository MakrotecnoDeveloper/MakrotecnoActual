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
    public string? EmpresaCliente { get; set; } = "NA";
    public string? CiudadCliente { get; set; } = "NA";
    public string? TelefonoCliente { get; set; } = "NA";
    public string? CorreoCliente { get; set; } = "NA";
    public string? DireccionCliente { get; set; } = "NA";
}
