using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("detallecompra", Schema = "dbo")]
public class DetalleCompra
{
        [Key]
        public int IdDetalleCompra { get; set; }

        public int IdCompra { get; set; }

        public string Codigo { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Stock { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal VNeto { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal VVenta { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal VTotal { get; set; }
        [ForeignKey("IdCompra")]
        public Compras Compra { get; set; }
}