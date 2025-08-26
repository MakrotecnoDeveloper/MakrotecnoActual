using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("MetodoPagos", Schema = "dbo")]
public class MetodoPagos
{
    [Key]
    public int IdMetodo { get; set; }
    public string Metodo {  get; set; }
}