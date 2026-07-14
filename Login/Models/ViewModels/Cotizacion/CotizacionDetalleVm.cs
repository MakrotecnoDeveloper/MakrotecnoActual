using Plataforma.Models;
using Plataforma.ViewModels.Cotizacion;

namespace Plataforma.ViewModels.Cotizacion;

public class CotizacionDetalleVm
{
    public int IdCotizacion { get; set; }
    public string NumeroCotizacion { get; set; } = string.Empty;
    public string EstadoCotizacion { get; set; } = string.Empty;

    public int IdCliente { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public int? CedulaCliente { get; set; }
    public string? TelefonoCliente { get; set; }
    public string? CorreoCliente { get; set; }
    public string? DireccionCliente { get; set; }

    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public DateTime? FechaAceptacion { get; set; }
    public DateTime? FechaRechazo { get; set; }
    public DateTime? FechaConversionVenta { get; set; }

    public int? IdVentaGenerada { get; set; }

    public string? ObservacionGeneral { get; set; }

    public decimal Subtotal { get; set; }
    public decimal IvaTotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public Empresas? Empresa { get; set; }

    public List<CotizacionDetalleItemVm> Detalles { get; set; } = new();

    public List<CotizacionSeguimientoItemVm> Seguimientos { get; set; } = new();

    public CotizacionSeguimientoVm NuevoSeguimiento { get; set; } = new();
}