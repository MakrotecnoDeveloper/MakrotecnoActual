using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("plataformas", Schema = "dbo")]
public class Plataformas
{
    [Key]
        public int IdPlataforma {  get; set; }
        public string? NombrePltf { get; set; }
}
