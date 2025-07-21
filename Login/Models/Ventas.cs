using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("ventas", Schema = "pruebas")]
public class Ventas
{
    [Key]
    public int IdVenta { get; set; }
    public int CedulaCliente { get; set; }
    public string MetodoPago { get; set; }
    public int Total { get; set; }
    public string EstadoVenta { get; set; }
    public int Cedula { get; set; }
    public DateTime FechaVenta { get; set; }
    public virtual ICollection<Pedidos> Pedidos { get; set; }
}
