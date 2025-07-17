using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("infopdv", Schema = "pruebas")]
public class Infopdv
{
    [Key]
    public int InfopdvId { get; set; }
    public string? Name { get; set; }
    public string? Id_Empresa { get; set; }
    public int Id_Sede { get; set; }
}