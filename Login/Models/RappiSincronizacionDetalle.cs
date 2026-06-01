using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Plataforma.Models;

public class RappiSincronizacionDetalle
{
    [Key]
    public int Id { get; set; }

    public int RappiSincronizacionId { get; set; }

    [ForeignKey(nameof(RappiSincronizacionId))]
    public RappiSincronizacion RappiSincronizacion { get; set; } = default!;

    [StringLength(100)]
    public string? SkuRappi { get; set; }

    [StringLength(255)]
    public string? ProductoId { get; set; }

    [StringLength(300)]
    public string? NombreProductoRappi { get; set; }

    [StringLength(50)]
    public string Estado { get; set; } = string.Empty;

    public string? Observacion { get; set; }
}