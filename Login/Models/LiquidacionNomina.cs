using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("LiquidacionesNomina", Schema = "dbo")]
public class LiquidacionNomina
{
    [Key]
    public int IdLiquidacion { get; set; }
    public int Cedula { get; set; }
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFin { get; set; }
    public DateTime FechaLiquidacion { get; set; } = DateTime.Now;
    public decimal TotalIngresos { get; set; }
    public decimal TotalDeducciones { get; set; }
    public decimal NetoPagar { get; set; }

    public Empleados Empleado { get; set; } = null!;
    public ICollection<DetalleLiquidacion> Detalles { get; set; } = new List<DetalleLiquidacion>();
}
