namespace Plataforma.Models;
public class ReporteFinancieroViewModel
{
    public DateTime Fecha { get; set; }
    public decimal Ingresos { get; set; }
    public decimal Costos { get; set; }
    public decimal Utilidad { get; set; }
    public decimal BalanceCaja { get; set; }
    public List<CierreCaja> Movimientos { get; set; }
}