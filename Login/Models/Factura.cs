using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("factura", Schema = "dbo")]
public class Factura
{
    [Key]
    public int IdFactura { get; set; }
    public string NumeroFactura { get; set; }
    public int IdVenta { get; set; }
    public DateTime FechaEmision { get; set; }
    public decimal SubTotal { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public string EstadoFactura { get; set; }
    [ForeignKey("IdVenta")]
    public virtual Ventas Venta { get; set; }
    public virtual ICollection<AdicionFactura> Adiciones { get; set; }
}

public class DetalleFacturaViewModel
{
    public Factura Factura { get; set; }
    public Clientes Cliente { get; set; } // puede ser null si no se encontró
    public Empresas Empresa { get; set; }
}