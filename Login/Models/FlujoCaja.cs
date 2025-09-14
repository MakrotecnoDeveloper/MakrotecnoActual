using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Plataforma.Models;
[Table("FlujoCaja", Schema = "dbo")]
public class FlujoCaja
{
    [Key]
    public int IdFlujoCaja { get; set; }
    public string? TipoMovimiento { get; set; }
    public string? Concepto { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal Monto { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal? Efectivo { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal? Transferencia { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal? GastosEfectivo { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal? GastosTransferencia { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal? Diferencia { get; set; }
    public int Cedula { get; set; }
    public DateTime Fecha { get; set; }
    public string? ConceptosJson { get; set; }

    // 🔑 Método helper para deserializar ConceptosJson
    public List<ConceptoServicioVM> GetConceptos()
    {
        if (string.IsNullOrWhiteSpace(ConceptosJson))
            return new List<ConceptoServicioVM>();

        try
        {
            return JsonSerializer.Deserialize<List<ConceptoServicioVM>>(ConceptosJson)
                   ?? new List<ConceptoServicioVM>();
        }
        catch
        {
            // Manejo simple si el JSON está corrupto
            return new List<ConceptoServicioVM>();
        }
    }
}
