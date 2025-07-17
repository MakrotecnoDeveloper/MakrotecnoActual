using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("diagnosticoproblema", Schema = "dbo")]
public class DiagnosticoProblema
{
    [Key]
    public int IdDiagProb { get; set; }
    public int IdOrden { get; set; }
    public string? ProblemaDetectado { get; set; }
    public string? Solucion { get; set; }
    public decimal Costo { get; set; }
    public decimal CostoInterno { get; set; }
    public string? Estado {  get; set; }
    public DateTime? FechaRegistro { get; set; }
    public int Cedula {  get; set; }
}
