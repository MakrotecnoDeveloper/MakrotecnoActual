using Microsoft.AspNetCore.Mvc.Rendering;
using Plataforma.ViewModels.Cotizacion;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.ViewModels.Cotizacion;

public class CotizacionCrearVm
{
    public int IdCotizacion { get; set; }

    public string NumeroCotizacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un cliente.")]
    public int IdCliente { get; set; }

    public int? CedulaCliente { get; set; }

    public string? NombreCliente { get; set; }

    public string? DocumentoCliente { get; set; }

    public string? TelefonoCliente { get; set; }

    public string? CorreoCliente { get; set; }

    public string? DireccionCliente { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaVencimiento { get; set; }

    public string? ObservacionGeneral { get; set; }

    public decimal Descuento { get; set; }

    public decimal Subtotal { get; set; }

    public decimal IvaTotal { get; set; }

    public decimal Total { get; set; }

    public List<SelectListItem> Clientes { get; set; } = new();

    public List<CotizacionDetalleItemVm> Detalles { get; set; } = new()
    {
        new CotizacionDetalleItemVm()
    };
}