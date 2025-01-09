namespace Plataforma.Models;
public class PedidoViewModel
{
    public Factura? Factura { get; set; }
    public List<Producto>? Productos { get; set; }
    public int? EstadoPDV { get; set; }
    public string? NombreCliente { get; set; }
}