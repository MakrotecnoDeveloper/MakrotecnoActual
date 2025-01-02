using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
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
