using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("ConceptosNomina", Schema = "dbo")]
public class ConceptoNomina
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Ingreso"; // Ingreso o Deducción
    public string? ServicioAsociado { get; set; } // "Tienda", "Recargas", etc
    public decimal? Porcentaje { get; set; } // Para comisiones
    public decimal? ValorFijo { get; set; } // Para deducciones
}
