using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("pedidos", Schema = "pruebas")]
public class Pedidos
{
    [Key]
    public int IdPedido { get; set; }
    public int IdVenta { get; set; }
    public string? CodProducto { get; set; }
    public decimal Cantidad { get; set; }
    public int ValorNeto { get; set; }
    public int ValorVenta { get; set; }
    public int InfopdvId { get; set; }
    public DateTime FechaRegistro { get; set; }
    public decimal SubTotal { get; set; }
    public virtual Ventas Venta { get; set; }
}
