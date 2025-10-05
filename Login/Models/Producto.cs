using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("productos", Schema = "dbo")]
public class Producto
{
        [Key]
        [Column("cod_producto")]
        public string? Cod_Producto { get; set; }
        [Column("nombreProducto")]
        public string? NombreProducto { get; set; }
        [Column("cantidadProducto")]
        public decimal CantidadProducto { get; set; }
        [Column("valorNetoProducto")]
        public decimal? ValorNetoProducto { get; set; }
        [Column("valorVentaProducto")]
        public decimal? ValorVentaProducto { get; set; }
        [Column("valorUnidad")]
        public int ValorUnidad {  get; set; }
        [Column("id_empresa")]
        public string? ID_Empresa { get; set; }
        [Column("estado")]
        public int Estado {  get; set; }
        [Column("Ubicacion")]
        public string? Ubicacion { get; set; }
        [Column("idCatePro")]
        public int IdCatepro {  get; set; }
        public int idProveedor { get; set; }
        [Column("imagenPath")]
        public string? ImagenPath { get; set; }
        public string? AutenticidadProducto { get; set; }
        public string? CondicionProducto { get; set; }
    public ICollection<InventarioSede> Inventarios { get; set; } = new List<InventarioSede>();
}
