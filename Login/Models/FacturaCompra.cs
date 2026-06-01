using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
public class FacturaCompra
{
    [Key]
    public int IdFacturaCompra { get; set; }

    [Required]
    public int IdProveedor { get; set; }

    public Proveedores? Proveedor { get; set; }

    public string? PrefijoFactura { get; set; }

    [Required]
    public string NumeroFactura { get; set; } = string.Empty;

    public DateTime FechaCompra { get; set; }

    [Required]
    public string RutaArchivo { get; set; } = string.Empty;

    public string? NombreArchivoOriginal { get; set; }

    public string? Observacion { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public bool Activo { get; set; } = true;

    public DateTime? FechaInactivacion { get; set; }

    public string? MotivoInactivacion { get; set; }

    public string? UsuarioInactivacion { get; set; }

    public bool ArchivoConservado { get; set; } = true;
}