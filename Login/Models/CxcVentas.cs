using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("CxcVentas", Schema = "dbo")]
public class CxcVenta
{
    [Key]
    public int IdCxc { get; set; }

    [ForeignKey("Venta")]
    public int IdVenta { get; set; }

    public int IdCliente { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SaldoPendiente { get; set; }

    public string EstadoCxc { get; set; } = "Pendiente"; // Pendiente, Pagada, Vencida

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public virtual Ventas Venta { get; set; }
    public virtual ICollection<CxcPago> Pagos { get; set; }
}