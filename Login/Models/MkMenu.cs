using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("MkMenu", Schema = "pruebas")]
public class MkMenu
{
    [Key]
    public int Id { get; set; }
    public string? Nombre { get; set; }

}
