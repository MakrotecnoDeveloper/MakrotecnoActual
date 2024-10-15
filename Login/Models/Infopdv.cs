using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class Infopdv
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Id_Empresa { get; set; }
    public int Id_Sede { get; set; }
}