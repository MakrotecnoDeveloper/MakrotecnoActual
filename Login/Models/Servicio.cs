using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("servicio", Schema = "pruebas")]
public class Servicio
{
    [Key]
    public int IdServicio { get; set; }
    public string? DescripcionServicio { get; set; }
    public string? NombreServicio { get; set; }
}
