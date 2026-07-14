using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;

[Table("CotizacionSeguimientos", Schema = "dbo")]
public class CotizacionSeguimiento
{
    [Key]
    public int IdCotizacionSeguimiento { get; set; }

    public int IdCotizacion { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [StringLength(30)]
    public string EstadoAnterior { get; set; } = string.Empty;

    [StringLength(30)]
    public string EstadoNuevo { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Observacion { get; set; } = string.Empty;

    public int CedulaUsuario { get; set; }

    [StringLength(150)]
    public string? UsuarioRegistro { get; set; }

    public virtual Cotizacion Cotizacion { get; set; } = null!;
}