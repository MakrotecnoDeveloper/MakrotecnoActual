using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("OrdenServicios", Schema = "dbo")]
public class OrdenServicios
{
    [Key]
    public int IdOrden { get; set; }
    public int IdDispositivo { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string? ProblemaReportado { get; set; }
    public string? Estado { get; set; } = "Ingresada";
    public Dispositivos? Dispositivo { get; set; }
    public string? Observaciones { get; set; }
    public int Cedula {  get; set; }
}
