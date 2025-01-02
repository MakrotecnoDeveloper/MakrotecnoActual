using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class Empleado
{
    [Key]
    public int Cedula { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Genero { get; set; }
    public string? Correo { get; set; }
    public string? Rh { get; set; }
    public string? Celular { get; set; }
    public string? Contrasena { get; set; }
}
