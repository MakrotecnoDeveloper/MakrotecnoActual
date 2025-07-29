using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("CierreCaja", Schema = "dbo")]
public class CierreCaja
{
    [Key]
    public int IdCierreCaja { get; set; }
    public DateTime Fecha { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalVentas { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal EfectivoReal { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal Diferencia { get; set; }
    public int Cedula {  get; set; }
}
