using System.ComponentModel.DataAnnotations;
namespace Plataforma.Models;
public class PdvInfo
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }

    [StringLength(50)]
    public string? Id_Empresa { get; set; }
    public int Id_Sucursal { get; set; }
}