using System.ComponentModel.DataAnnotations;

namespace Plataforma.ViewModels.Cotizacion;

public class CotizacionDetalleItemVm
{
    public int IdCotizacionDetalle { get; set; }

    [Required]
    public string CodigoProducto { get; set; } = string.Empty;

    public string NombreProducto { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Cantidad { get; set; }

    public decimal ValorNeto { get; set; }

    public decimal ValorUnidad { get; set; }

    public decimal ValorVenta { get; set; }

    public decimal IvaPorcentaje { get; set; }

    public decimal IvaValor { get; set; }

    public decimal SubTotal { get; set; }

    public decimal TotalLinea { get; set; }

    public decimal StockDisponible { get; set; }

    public bool IncluidoEnVenta { get; set; } = true;

    public string? Observacion { get; set; }
}