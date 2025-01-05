using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class Producto
{
    [Key]
        public string? Cod_Producto { get; set; }
        public string? NombreProducto { get; set; }
        public decimal CantidadProducto { get; set; }
        public float ValorNetoProducto { get; set; }
        public float ValorVentaProducto { get; set; }
        public int ValorUnidad {  get; set; }
        public string? ID_Empresa { get; set; }
        public string? Categoria { get; set; }
        public int Estado {  get; set; }
        public string? Ubicacion { get; set; }
}
