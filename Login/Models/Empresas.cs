//Representacion de la tabla Usuario de la base de datos
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class Empresas
{
    [Key]
    public string? id_empresa { get; set; }
    public string? nombre { get; set; }
    public string? pais { get; set; }
    public string? direccion { get; set; }
    public string? telefono { get; set; }
}