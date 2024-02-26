using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models
{
    public partial class Producto
    {
        [Key]
        public string? cod_producto { get; set; }
        public string? nombreProducto { get; set; }
        public int cantidadProducto { get; set; }
        public float valorNetoProducto { get; set; }
        public float valorVentaProducto { get; set; }
        public string? id_empresa { get; set; }
        public string? categoria { get; set; }
    }
}
