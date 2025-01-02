using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class GananciaPedido
{
    [Key]
    public int IdGP {  get; set; }
    public int Cod_pedido { get; set; }
    public int? Ganancia { get; set; }
}
