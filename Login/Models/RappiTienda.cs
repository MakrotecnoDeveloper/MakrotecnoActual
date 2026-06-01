using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Plataforma.Models;

public class RappiTienda
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string IdTiendaRappi { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string NombreTiendaRappi { get; set; } = string.Empty;

    public int SedeId { get; set; }

    [ForeignKey(nameof(SedeId))]
    public Sede Sede { get; set; } = default!;

    public bool Activa { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}