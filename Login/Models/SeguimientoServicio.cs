using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("seguimientoservicio", Schema = "dbo")]
public class SeguimientoServicio
{
    [Key]
    public int IdSeguiServ { get; set; }
    public int IdOrden { get; set; }
    public DateTime FechaSeguimiento { get; set; }
    public string? RespuestaCliente { get; set; }
    public string? Estado { get; set; }
}
