namespace Plataforma.Models
{
    public class DetalleConceptoEmpleado
    {
        public int IdDetalle { get; set; }
        public int Cedula { get; set; } = 0!;
        public int IdConcepto { get; set; }
        public decimal? PorcentajePersonalizado { get; set; }
        public decimal? ValorFijoPersonalizado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Estado { get; set; }
        public string? Observacion { get; set; }
        public DateTime FechaCreacion { get; set; }

        public virtual Empleados Empleado { get; set; } = null!;
        public virtual ConceptoNomina ConceptoNomina { get; set; } = null!;
    }
}