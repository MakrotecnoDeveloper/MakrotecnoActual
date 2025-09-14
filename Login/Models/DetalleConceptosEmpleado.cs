using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("DetalleConceptosEmpleado", Schema = "dbo")]
public class DetalleConceptosEmpleado
{
    [Key]
    public int IdDetalle { get; set; }
    public int Cedula { get; set; }
    public int IdConcepto { get; set; }
    [ForeignKey("Cedula")]
    public Empleados Empleado { get; set; } = null!;
    [ForeignKey("IdConcepto")]
    public ConceptoNomina Concepto { get; set; } = null!;
}
