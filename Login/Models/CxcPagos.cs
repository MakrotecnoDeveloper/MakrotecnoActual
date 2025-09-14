using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("CxcPagos", Schema = "dbo")]
public class CxcPago
{
    [Key]
    public int IdPago { get; set; }

    [ForeignKey("CxcVenta")]
    public int IdCxc { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MontoPago { get; set; }

    public DateTime FechaPago { get; set; } = DateTime.Now;

    public string Observacion { get; set; }

    public virtual CxcVenta CxcVenta { get; set; }
}