using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;

[Table("CuentasCobro", Schema = "dbo")]
public class CuentaCobro
{
    [Key]
    public int IdCuentaCobro { get; set; }

    [Required]
    [StringLength(50)]
    public string NumeroCuentaCobro { get; set; } = string.Empty;

    [StringLength(50)]
    public string CodigoDocumento { get; set; } = "DCCC-000-01";

    [StringLength(20)]
    public string VersionDocumento { get; set; } = "1";

    public DateTime? FechaAprobacionDocumento { get; set; }

    public int IdFactura { get; set; }
    public int IdVenta { get; set; }
    public int IdCliente { get; set; }
    public int IdCuentaBancariaEmpresa { get; set; }

    public DateTime FechaCuentaCobro { get; set; } = DateTime.Now;

    [StringLength(100)]
    public string? Ciudad { get; set; }

    [StringLength(200)]
    public string? NombreCliente { get; set; }

    [StringLength(50)]
    public string? DocumentoCliente { get; set; }

    [StringLength(250)]
    public string? DireccionCliente { get; set; }

    [StringLength(100)]
    public string? TelefonoCliente { get; set; }

    [StringLength(150)]
    public string? CorreoCliente { get; set; }

    [StringLength(100)]
    public string? BancoCliente { get; set; }

    [StringLength(1000)]
    public string? DetalleServicio { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorTotalServicio { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RetencionFuente { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OtrasDeducciones { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorNetoPagar { get; set; }

    [StringLength(1000)]
    public string? Declaracion { get; set; }

    [StringLength(50)]
    public string? CedulaContratista { get; set; }

    [StringLength(100)]
    public string? CiudadExpedicionContratista { get; set; }

    [StringLength(300)]
    public string? FirmaPath { get; set; }

    [StringLength(30)]
    public string EstadoCuentaCobro { get; set; } = "Pendiente";

    [Required]
    [StringLength(50)]
    public string IdEmpresa { get; set; } = string.Empty;

    public int SedeId { get; set; }
    public int InfopdvId { get; set; }
    public int CedulaUsuario { get; set; }

    [StringLength(150)]
    public string? CreadoPor { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public virtual Factura Factura { get; set; } = null!;
    public virtual Ventas Venta { get; set; } = null!;
    public virtual Clientes Cliente { get; set; } = null!;
    public virtual CuentaBancariaEmpresa CuentaBancariaEmpresa { get; set; } = null!;

    public virtual ICollection<CuentaCobroDetalle> Detalles { get; set; } = new List<CuentaCobroDetalle>();
}