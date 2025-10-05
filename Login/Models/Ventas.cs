using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("ventas", Schema = "dbo")]
public class Ventas
{
    [Key]
    public int IdVenta { get; set; }
    public int IdCliente { get; set; }
    public int MetodoPago { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }
    public string EstadoVenta { get; set; }
    public int Cedula { get; set; }
    public DateTime FechaVenta { get; set; }
    public virtual ICollection<Pedidos> Pedidos { get; set; }
    public int CedulaCliente { get; set; }
    public string Conceptos { get; set; }
    [ForeignKey(nameof(MetodoPago))]
    public MetodoPagos MetodoPagos { get; set; } = default!;
}
