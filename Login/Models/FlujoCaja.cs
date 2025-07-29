using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("FlujoCaja", Schema = "dbo")]
public class FlujoCaja
{
    [Key]
    public int IdFlujoCaja { get; set; }
    public string? TipoMovimiento { get; set; }
    public string? Concepto { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal Monto { get; set; }
    public int Cedula { get; set; }
    public DateTime Fecha { get; set; }
}
