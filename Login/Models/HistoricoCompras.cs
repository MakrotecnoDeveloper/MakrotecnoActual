using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
public class HistoricoCompras
{
        [Key]
        public int IdHC { get; set; }

        [Required]
        [StringLength(50)] // Limitar el tamaño del campo a 50 caracteres
        public required string Nit { get; set; }

        [StringLength(255)] // Limitar el tamaño del campo a 255 caracteres
        public required string Cod_Producto { get; set; }

        [Column(TypeName = "decimal(10,2)")] // Define precisión para la base de datos
        public decimal ValorU { get; set; }

        //[Column(TypeName = "decimal(5,2)")] // Define precisión para la base de datos
        [Column(TypeName = "decimal(10,2)")]
        public decimal Stock { get; set; }

        [Column(TypeName = "decimal(10,2)")] // Define precisión para la base de datos
        public decimal ValorTotal { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now; // Valor por defecto

        public int Estado { get; set; } = 1; // Valor por defecto
        public int cod_factura { get; set; }
}