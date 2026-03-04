using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;

[Table("CxcVentas", Schema = "dbo")]
public class CxcVentas
{
    [Key]
    public int IdCxc { get; set; }

    [ForeignKey(nameof(Venta))]
    public int IdVenta { get; set; }

    public int IdCliente { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SaldoPendiente { get; set; }

    // Abierta | Parcial | Pagada | Anulada
    public string EstadoCxc { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaVencimiento { get; set; }

    public string? Observacion { get; set; }

    // === NAVEGACIONES ===
    public virtual Ventas Venta { get; set; }

    public virtual ICollection<CxcPagos> Pagos { get; set; }
}
