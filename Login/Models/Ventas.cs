using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("ventas", Schema = "pruebas")]
public class Ventas
{
    [Key]
    public int Id_venta { get; set; }
    public int VentaTotal { get; set; }
    public int VentaMakrotecno { get; set; }
    public int NetoMakrotecno { get; set; }
    public int VentaRecargas { get; set; }
    public int VentaTienda { get; set; }
    public int VentaPasivos { get; set; }
    public DateTime FechaVenta { get; set; }
}
