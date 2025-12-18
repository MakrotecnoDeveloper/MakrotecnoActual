using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
public class Agendamientos
{
    [Key]
    public int Id { get; set; }
    public string Cod_Producto { get; set; }
    public DateTime Fecha { get; set; }
    public string NombreCliente { get; set; }
    public string CelularCliente { get; set; }
    public string Estado { get; set; }
}
