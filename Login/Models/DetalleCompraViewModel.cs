namespace Plataforma.Models;

public class DetalleCompraViewModel
{
    public string Codigo { get; set; } = string.Empty;

    public decimal Stock { get; set; }

    public decimal ValorUnidad { get; set; }

    public decimal ValorVentaProducto { get; set; }

    public decimal VTotal { get; set; }
}
