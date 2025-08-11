using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("Dispositivos", Schema = "dbo")]
public class Dispositivos
{
    [Key]
    public int IdDispositivo { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? IMEI { get; set; }
    public int CedulaCliente { get; set; }
    public int IdCliente { get; set; }
    public Clientes Cliente { get; set; }
    public string? Clave { get; set; }
    public string? Patron { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string? Detalle { get; set; }
    public int TipoDispositivo { get; set; }
}
