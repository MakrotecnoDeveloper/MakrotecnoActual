using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("gananciapedido", Schema = "pruebas")]
public class GananciaPedido
{
    [Key]
    public int IdGP {  get; set; }
    public int Cod_pedido { get; set; }
    public int? Ganancia { get; set; }
}
