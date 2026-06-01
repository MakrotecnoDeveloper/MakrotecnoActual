using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Plataforma.Models;

public class RappiSincronizacion
{
    [Key]
    public int Id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [StringLength(100)]
    public string? UsuarioId { get; set; }

    [StringLength(300)]
    public string? ArchivoOriginal { get; set; }

    [StringLength(300)]
    public string? ArchivoGenerado { get; set; }

    public int TotalFilas { get; set; }

    public int TotalActualizados { get; set; }

    public int TotalSinAsociar { get; set; }

    public string? Observaciones { get; set; }

    public ICollection<RappiSincronizacionDetalle> Detalles { get; set; } = new List<RappiSincronizacionDetalle>();
}