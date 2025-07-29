using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("tipocargo", Schema = "dbo")]
public class TipoCargo
{
    [Key]
    public int Id_tipo { get; set; }
    public string? NombreCargo { get; set; }
    public string? DescripcionCargo { get; set; }
    public string? Id_empresa { get; set; }
}
