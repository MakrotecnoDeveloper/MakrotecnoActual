using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Plataforma.Models;

public class RappiCategoriaMapeo
{
    [Key]
    public int Id { get; set; }

    public int IdCateProducto { get; set; }

    [ForeignKey(nameof(IdCateProducto))]
    public CategoriaProductos CategoriaProducto { get; set; } = default!;

    [Required]
    [StringLength(500)]
    public string CategoriaRappi { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}