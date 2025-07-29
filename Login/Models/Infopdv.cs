using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("infopdv", Schema = "dbo")]
public class Infopdv
{
    [Key]
    public int InfopdvId { get; set; }
    public string? NombreInfoPDV { get; set; }
    public int Id_Sede { get; set; }
}