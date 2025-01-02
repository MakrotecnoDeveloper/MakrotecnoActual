using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;
public class CategoriaProductos
{
    [Key]
    public int IdCateProducto { get; set; }
    public string? Descripcion {  get; set; }
    public int IdServicio { get; set; }

}
