using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("syncpdv", Schema = "pruebas")]
public class Syncpdv
{
    [Key]
    public int Idsync { get; set; }
    public int InfopdvId { get; set; }
    public int Estado { get; set; }
    public DateTime FechaEstado { get; set; }
    public int Cedula {  get; set; }
}