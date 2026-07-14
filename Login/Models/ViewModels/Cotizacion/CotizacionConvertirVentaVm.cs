using Plataforma.ViewModels.Cotizacion;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.ViewModels.Cotizacion;

public class CotizacionConvertirVentaVm
{
    public int IdCotizacion { get; set; }

    public string NumeroCotizacion { get; set; } = string.Empty;

    public int IdCliente { get; set; }

    public int? CedulaCliente { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ObservacionVenta { get; set; }

    public List<CotizacionDetalleItemVm> Detalles { get; set; } = new();

    public decimal TotalSeleccionado { get; set; }
}