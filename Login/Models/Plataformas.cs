using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class Plataformas
{
    [Key]
        public int IdPlataforma {  get; set; }
        public string? NombrePltf { get; set; }
}
