using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
public class Servicio
{
    [Key]
    public int IdServicio { get; set; }
    public string? DescripcionServicio { get; set; }
    public string? NombreServicio { get; set; }
}
