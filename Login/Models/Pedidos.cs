using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("pedidos", Schema = "dbo")]
public class Pedidos
{
    [Key]
    public int IdPedido { get; set; }
    public int IdVenta { get; set; }
    public string? Codigo { get; set; }
    public decimal Stock { get; set; }
    public int VNeto { get; set; }
    public int VVenta { get; set; }
    public int InfopdvId { get; set; }
    public DateTime FechaRegistro { get; set; }
    public decimal SubTotal { get; set; }
    public virtual Ventas Venta { get; set; }
}
