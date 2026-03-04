using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;

[Table("CxcPagos", Schema = "dbo")]
public class CxcPagos
{
    [Key]
    public int IdPago { get; set; }

    [ForeignKey(nameof(CxcVenta))]
    public int IdCxc { get; set; }

    [ForeignKey(nameof(MetodoPagos))]
    public int IdMetodo { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MontoPago { get; set; }

    public DateTime FechaPago { get; set; } = DateTime.Now;

    public string? Observacion { get; set; }

    // === NAVEGACIONES ===
    public virtual CxcVentas CxcVenta { get; set; }

    public virtual MetodoPagos MetodoPagos { get; set; }
}
