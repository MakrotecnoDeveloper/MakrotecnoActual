using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("DistribucionUtilidad", Schema = "dbo")]
public class DistribucionUtilidad
{
    [Key]
    public int IdDistribucion { get; set; }

    [Required]
    public int IdUnidadNegocio { get; set; }

    [Required]
    public int CedulaEmpleado { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal Porcentaje { get; set; }

    // Relaciones
    [ForeignKey("IdUnidadNegocio")]
    public UnidadNegocio UnidadNegocio { get; set; }
}