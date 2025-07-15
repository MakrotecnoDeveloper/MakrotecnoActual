namespace Plataforma.Models;
public class ProductoViewModel
{
    public string? Codigo { get; set; }
    public string? Stock { get; set; }
    public string? UnidadMedida {  get; set; }
    public int VNeto { get; set; }
    public int VVenta { get; set; }
    public decimal VTotal { get; set; }
}