using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("empleadoempresa", Schema = "dbo")]
public class EmpleadoEmpresa
{
    [Key]
    public int Id_empleadoE {  get; set; }
    public int Cedula { get; set; }
    public string? Id_empresa { get; set; }
}
