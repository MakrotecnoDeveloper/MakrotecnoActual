using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("factura", Schema = "pruebas")]
public class Factura
{
    [Key]
    public int Cod_factura { get; set; }
    public int Cedula_cliente { get; set; }
    public int Cedula { get; set; }
    public DateTime FechaVenta { get; set; }
    public string? Estado { get; set; }
    public string? TipoFactura { get; set; }
}
