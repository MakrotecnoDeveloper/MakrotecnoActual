using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("detallecompra", Schema = "dbo")]
public class DetalleCompra
{
        [Key]
        public int IdDetalleCompra { get; set; }

        [Required]
        public int IdCompra { get; set; }

        public string CodProducto { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Cantidad { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorU { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorTotal { get; set; }
        [ForeignKey("IdCompra")]
        public Compras Compra { get; set; }
}