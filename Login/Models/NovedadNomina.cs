namespace Plataforma.Models
{
    public class NovedadNomina
    {
        public int IdNovedad { get; set; }
        public int Cedula { get; set; } = 0!;
        public int IdConcepto { get; set; }
        public int IdPeriodo { get; set; }
        public DateTime FechaNovedad { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? BaseValor { get; set; }
        public decimal? PorcentajeAplicado { get; set; }
        public decimal Valor { get; set; }
        public string? DocumentoOrigen { get; set; }
        public string Origen { get; set; } = "Manual";
        public string Estado { get; set; } = "Pendiente";
        public string? Observacion { get; set; }
        public DateTime FechaCreacion { get; set; }

        public virtual Empleados Empleado { get; set; } = null!;
        public virtual ConceptoNomina ConceptoNomina { get; set; } = null!;
        public virtual PeriodoNomina PeriodoNomina { get; set; } = null!;
    }
}