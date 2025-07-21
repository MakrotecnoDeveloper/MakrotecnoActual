using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("dispositivo", Schema = "dbo")]
public class Dispositivo
{
    [Key]
    public int IdDispositivo { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? IMEI { get; set; }
    public string? Accesorios { get; set; }
    public int CedulaCliente { get; set; }
    public Cliente Cliente { get; set; }
}
