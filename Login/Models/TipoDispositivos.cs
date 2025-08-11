using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("TipoDispositivos", Schema = "dbo")]
public class TipoDispositivos
{
    [Key]
    public int TipoDispositivo { get; set; }
    public string? Descripcion { get; set; }
}
