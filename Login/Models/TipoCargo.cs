using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class TipoCargo
{
    [Key]
    public int Id_tipo { get; set; }
    public string? NombreCargo { get; set; }
    public string? DescripcionCargo { get; set; }
    public string? Id_empresa { get; set; }
}
