using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class Empresas
{
    [Key]
    public string? Id_empresa { get; set; }
    public string? Nombre { get; set; }
    public string? Pais { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
}