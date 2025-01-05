using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class Pedidos
{
    [Key]
    public int Cod_pedido { get; set; }
    public int Cod_factura { get; set; }
    public string? Cod_producto { get; set; }
    public decimal Cantidad { get; set; }
    public int ValorNeto { get; set; }
    public int ValorVenta { get; set; }
    public string? Estado { get; set; }
    public int InfopdvId { get; set; }
    public DateTime FechaIngreso { get; set; }
}
