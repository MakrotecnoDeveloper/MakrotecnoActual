using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class EmpleadoEmpresa
{
    [Key]
    public int Id_empleadoE {  get; set; }
    public int Cedula { get; set; }
    public string? Id_empresa { get; set; }
}
