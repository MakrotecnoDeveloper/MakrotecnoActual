using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models;

public partial class Syncpdv
{
    [Key]
    public int Idsync { get; set; }
    public int Id { get; set; }
    public int estado { get; set; }
    public DateTime fechaEstado { get; set; }
}