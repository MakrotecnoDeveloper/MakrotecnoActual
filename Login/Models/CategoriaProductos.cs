using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("categoriaproductos", Schema = "pruebas")]
public class CategoriaProductos
{
    [Key]
    public int IdCateProducto { get; set; }
    public string? Descripcion {  get; set; }
    public int IdServicio { get; set; }

}
