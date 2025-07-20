using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("MkPermisosMenu", Schema = "pruebas")]
public class MkPermisosMenu
{
    [Key]
    public int Id { get; set; }
    public int Id_TipoCargo { get; set; }
    public int Id_Menu { get; set; }
    public bool Estado {  get; set; }

}
