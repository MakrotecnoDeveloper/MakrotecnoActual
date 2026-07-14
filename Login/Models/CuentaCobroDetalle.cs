using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;

[Table("CuentasCobroDetalle", Schema = "dbo")]
public class CuentaCobroDetalle
{
    [Key]
    public int IdCuentaCobroDetalle { get; set; }

    public int IdCuentaCobro { get; set; }

    [StringLength(50)]
    public string? CodigoProducto { get; set; }

    [Required]
    [StringLength(250)]
    public string NombreProducto { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cantidad { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorUnitario { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal IvaValor { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalLinea { get; set; }

    public virtual CuentaCobro CuentaCobro { get; set; } = null!;
}