using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class Estadopdv
{
    [Key]
    public int Id_epdv { get; set; }
    public int Estado { get; set; }
    public int Id { get; set; }
    public DateTime FechaProceso { get; set; }
}