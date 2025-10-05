using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("HistOrdSer", Schema = "dbo")]
public class HistOrdSer
{
    [Key]
    public int IdDiagProb { get; set; }
    public int IdOrden { get; set; }
    public string? ReparacionDet { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public string? Cod_Producto { get; set; }
    public int Cedula {  get; set; }
    public decimal? ValorNetoProducto {  get; set; }
    public decimal? ValorVentaProducto { get; set; }
    public string? AutenticidadProducto { get; set; }
    public string? CondicionProducto { get; set; }
    public int IdProveedor { get; set; }
    public int Stock {  get; set; }
}
