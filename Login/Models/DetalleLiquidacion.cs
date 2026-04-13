namespace Plataforma.Models
{
    public class DetalleLiquidacion
    {
        public int IdDetalle { get; set; }
        public int IdLiquidacion { get; set; }
        public int IdConcepto { get; set; }
        public string NombreConcepto { get; set; } = null!;
        public string Naturaleza { get; set; } = null!;
        public decimal? BaseAplicada { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? PorcentajeAplicado { get; set; }
        public decimal? ValorUnitario { get; set; }
        public decimal Monto { get; set; }
        public string? OrigenDetalle { get; set; }
        public string? Observacion { get; set; }

        public virtual LiquidacionNomina LiquidacionNomina { get; set; } = null!;
        public virtual ConceptoNomina ConceptoNomina { get; set; } = null!;
    }
}