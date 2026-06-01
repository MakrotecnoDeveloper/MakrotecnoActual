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
    public decimal? VNeto { get; set; }
    public decimal? VUnidad { get; set; }
    public decimal VVenta { get; set; } = 0;
    public int InfopdvId { get; set; }
    public DateTime FechaRegistro { get; set; }
    public decimal SubTotal { get; set; }
    public virtual Ventas Venta { get; set; }
    public decimal? IvaPorcentaje { get; set; }
    public decimal? IvaValor { get; set; }
    [ForeignKey("Codigo")]
    public virtual Producto Producto { get; set; }
}

public class ProductoVentaDto
{
    public string codigo { get; set; } = "";
    public string nombre { get; set; } = "";
    public decimal? valorNeto { get; set; }
    public decimal? valorUnidad { get; set; }
    public decimal? valorVenta { get; set; }
    public decimal? stockDisponible { get; set; }
}
