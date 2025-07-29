using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("sede", Schema = "dbo")]
public class Sede
{
    [Key]
    public int Id_sede { get; set; }
    public string? Id_empresa { get; set; }
    public string? NombreSede { get; set; }
    public string? Ciudad { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
}