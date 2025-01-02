using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class Ganancias
{
    [Key]
    public int Id_ganancias { get; set; }
    public int GananciaMakrotecno { get; set; }
    public int GananciaTotal { get; set; }
    public int GananciaMaria { get; set; }
    public int GananciaVictor { get; set; }
    public int GananciaTeresa { get; set; }
    public DateTime FechaGanancia { get; set; }
}