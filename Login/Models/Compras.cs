using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("compras", Schema = "dbo")]
public class Compras
{
        [Key]
        public int IdCompra { get; set; }

        [Required]
        public int IdProveedor { get; set; }

        [Column(TypeName = "decimal(10,2)")] // Define precisión para la base de datos
        public decimal ValorTotal { get; set; }

        [Required]
        public DateTime? FechaCompra { get; set; } = DateTime.Now; // Valor por defecto

        public int Estado { get; set; } = 1; // Valor por defecto
        public string CodFacturaExterno { get; set; }
        public int Iva {  get; set; }
        [Column(TypeName = "decimal(10,2)")] // Define precisión para la base de datos
        public decimal DescuentoFactura { get; set; }
        public virtual ICollection<DetalleCompra> Detalles { get; set; }
}