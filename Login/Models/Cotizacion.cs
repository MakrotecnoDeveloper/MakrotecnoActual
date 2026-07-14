using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;

[Table("Cotizaciones", Schema = "dbo")]
public class Cotizacion
{
    [Key]
    public int IdCotizacion { get; set; }

    [Required]
    [StringLength(50)]
    public string NumeroCotizacion { get; set; } = string.Empty;

    public int IdCliente { get; set; }

    public int? CedulaCliente { get; set; }

    [StringLength(200)]
    public string? NombreCliente { get; set; }

    [StringLength(50)]
    public string? DocumentoCliente { get; set; }

    [StringLength(100)]
    public string? TelefonoCliente { get; set; }

    [StringLength(150)]
    public string? CorreoCliente { get; set; }

    [StringLength(250)]
    public string? DireccionCliente { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaVencimiento { get; set; }

    public DateTime? FechaAceptacion { get; set; }

    public DateTime? FechaRechazo { get; set; }

    public DateTime? FechaConversionVenta { get; set; }

    public int? IdVentaGenerada { get; set; }

    [StringLength(30)]
    public string EstadoCotizacion { get; set; } = "Pendiente";

    [StringLength(500)]
    public string? ObservacionGeneral { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal IvaTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Descuento { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [Required]
    [StringLength(50)]
    public string IdEmpresa { get; set; } = string.Empty;

    public int SedeId { get; set; }

    public int InfopdvId { get; set; }

    public int CedulaUsuario { get; set; }

    [StringLength(150)]
    public string? CreadoPor { get; set; }

    public bool Activo { get; set; } = true;

    public virtual Clientes Cliente { get; set; } = null!;

    public virtual Ventas? VentaGenerada { get; set; }

    public virtual ICollection<CotizacionDetalle> Detalles { get; set; } = new List<CotizacionDetalle>();

    public virtual ICollection<CotizacionSeguimiento> Seguimientos { get; set; } = new List<CotizacionSeguimiento>();
}