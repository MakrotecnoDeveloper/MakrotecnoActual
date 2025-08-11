using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("ModeloGanancia", Schema = "dbo")]
public class ModeloGanancia
{
    [Key]
    public int IdModeloGanancia { get; set; }

    [Required]
    [StringLength(50)]
    public string id_empresa { get; set; }

    [Required]
    [StringLength(100)]
    public string NombreModelo { get; set; }

    public string Descripcion { get; set; }

    // Relaciones
    [ForeignKey("id_empresa")]
    public Empresas Empresa { get; set; }
}
    