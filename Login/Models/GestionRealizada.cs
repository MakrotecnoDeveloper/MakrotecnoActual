using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("gestionrealizada", Schema = "dbo")]
public class GestionRealizada
{
    [Key]
    public int IdGR { get; set; }
    public int IdOrden { get; set; }
    public DateTime FechaEntrega { get; set; }
    public decimal CostoTotal { get; set; }
}
