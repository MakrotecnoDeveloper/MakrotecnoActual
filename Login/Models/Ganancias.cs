using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("ganancias", Schema = "dbo")]
public class Ganancias
{
    [Key]
    public int IdGanancia { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Ingresos { get; set; }
    public decimal Costos { get; set; }
    public decimal Utilidad { get; set; }
    public int Cedula {  get; set; }
}