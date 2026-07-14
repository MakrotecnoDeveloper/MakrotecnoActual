namespace Plataforma.ViewModels.Cotizacion;

public class CotizacionIndexVm
{
    public int IdCotizacion { get; set; }
    public string NumeroCotizacion { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string EstadoCotizacion { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int? IdVentaGenerada { get; set; }
}