namespace Plataforma.ViewModels.CuentasCobro;

public class CuentaCobroDetalleItemVm
{
    public string? CodigoProducto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal IvaValor { get; set; }
    public decimal TotalLinea { get; set; }
}