namespace Plataforma.Models;
public class PedidosViewModel
{
    public int IdVenta { get; set; }
    public List<Pedidos> Productos { get; set; } = new();
}