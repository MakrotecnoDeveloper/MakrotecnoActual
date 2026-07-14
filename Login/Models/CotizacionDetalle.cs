using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;

[Table("CotizacionDetalles", Schema = "dbo")]
public class CotizacionDetalle
{
    [Key]
    public int IdCotizacionDetalle { get; set; }

    public int IdCotizacion { get; set; }

    [Required]
    [StringLength(50)]
    public string CodigoProducto { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string NombreProducto { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cantidad { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorNeto { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorUnidad { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorVenta { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal IvaPorcentaje { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal IvaValor { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalLinea { get; set; }

    public bool IncluidoEnVenta { get; set; } = true;

    [StringLength(500)]
    public string? Observacion { get; set; }

    public virtual Cotizacion Cotizacion { get; set; } = null!;

    [ForeignKey(nameof(CodigoProducto))]
    public virtual Producto Producto { get; set; } = null!;
}