using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("ordenservicio", Schema = "dbo")]
public class OrdenServicio
{
    [Key]
    public int IdOrden { get; set; }
    public int IdDispositivo { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string? ProblemaReportado { get; set; }
    public string? Estado { get; set; }
    public DateTime FechaModificacion { get; set; }
}
