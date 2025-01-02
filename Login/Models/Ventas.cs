using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
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
