using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("MKPromociones", Schema = "dbo")]

public class MkPromociones
{

    [Key]
    public int Id { get; set; }

    public string NombreArchivo { get; set; }

    public bool? Estado { get; set; }

    public string? Descripcion { get; set; }
}
