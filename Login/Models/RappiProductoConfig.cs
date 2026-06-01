using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Plataforma.Models;

public class RappiProductoConfig
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ProductoId { get; set; } = string.Empty;

    [ForeignKey(nameof(ProductoId))]
    public Producto Producto { get; set; } = default!;

    [StringLength(100)]
    public string? SkuRappi { get; set; }

    [StringLength(300)]
    public string? NombreRappi { get; set; }

    public string? DescripcionRappi { get; set; }

    [StringLength(200)]
    public string? MarcaRappi { get; set; }

    [StringLength(50)]
    public string? Ean { get; set; }

    public bool EsPesable { get; set; } = false;

    public bool EsPreempaquetado { get; set; } = true;

    [Column(TypeName = "decimal(18,2)")]
    public decimal CantidadPresentacion { get; set; } = 1;

    [StringLength(50)]
    public string UnidadMedidaRappi { get; set; } = "Und (unidades)";

    public bool PublicarEnRappi { get; set; } = true;

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}