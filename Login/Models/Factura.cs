using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("factura", Schema = "pruebas")]
public class Factura
{
    [Key]
    public int IdFactura { get; set; }
    public int NumeroFactura { get; set; }
    public int IdVenta { get; set; }
    public DateTime FechaEmision { get; set; }
    public decimal SubTotal { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public string EstadoFactura { get; set; }
    [ForeignKey("IdVenta")]
    public virtual Ventas Venta { get; set; }
}
