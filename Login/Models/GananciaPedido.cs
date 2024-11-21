using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class GananciaPedido
{
    [Key]
    public int idGP {  get; set; }
    public int cod_pedido { get; set; }
    public int? ganancia { get; set; }
}
