using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("Contratos", Schema = "dbo")]
public class Contratos
{
    [Key]
    public int IdContrato { get; set; }
    public int Cedula { get; set; }
    public string TipoContrato { get; set; } = "";
    public decimal? SalarioBase { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; } = true;

    public Empleados Empleado { get; set; } = null!;

}
