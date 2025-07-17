using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("empresas", Schema = "pruebas")]
public class Empresas
{
    [Key]
    public string? Id_empresa { get; set; }
    public string? Nombre { get; set; }
    public string? Pais { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
}