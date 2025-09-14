using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("DetalleLiquidacion", Schema = "dbo")]
public class DetalleLiquidacion
{
    [Key]
    public int IdDetalle { get; set; }
    public int IdLiquidacion { get; set; }
    public int IdConcepto { get; set; }
    public decimal Monto { get; set; }

    public LiquidacionNomina Liquidacion { get; set; } = null!;
    public ConceptoNomina Concepto { get; set; } = null!;
}
