using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class Syncpdv
{
    [Key]
    public int Idsync { get; set; }
    public int InfopdvId { get; set; }
    public int Estado { get; set; }
    public DateTime FechaEstado { get; set; }
    public int Cedula {  get; set; }
}