using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;

[Table("InventarioSede", Schema = "dbo")]
public class InventarioSede
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdInventario { get; set; }

    // FK → Producto
    public string ProductoId { get; set; }
    [ForeignKey(nameof(ProductoId))]
    public Producto Producto { get; set; } = default!;

    // FK → Sede
    public int SedeId { get; set; }
    [ForeignKey(nameof(SedeId))]
    public Sede Sede { get; set; } = default!;

    public decimal Cantidad { get; set; }
    public int? PrecioUnitario { get; set; }
    public DateTime ActualizadoEn { get; set; }

    // FK → Empleado
    public int Cedula { get; set; }
    [ForeignKey(nameof(Cedula))]
    public Empleados Empleado { get; set; } = default!;
}
