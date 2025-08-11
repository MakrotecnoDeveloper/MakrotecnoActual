using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("UnidadNegocio", Schema = "dbo")]
public class UnidadNegocio
{
    [Key]
    public int IdUnidadNegocio { get; set; }

    [Required]
    [StringLength(50)]
    public string id_empresa { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; }

    [StringLength(300)]
    public string Descripcion { get; set; }

    // Relaciones
    [ForeignKey("id_empresa")]
    public Empresas Empresa { get; set; }

    public ICollection<DistribucionUtilidad> Distribuciones { get; set; }
}