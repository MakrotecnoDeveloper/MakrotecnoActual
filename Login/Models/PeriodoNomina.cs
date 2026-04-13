using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models
{
    public class PeriodoNomina
    {
        public int IdPeriodo { get; set; }
        public string Descripcion { get; set; } = null!;
        public string TipoPeriodo { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime? FechaPago { get; set; }
        public string Estado { get; set; } = "Abierto";
        public DateTime FechaCreacion { get; set; }

        public virtual ICollection<NovedadNomina> NovedadesNomina { get; set; } = new List<NovedadNomina>();
        public virtual ICollection<LiquidacionNomina> LiquidacionesNomina { get; set; } = new List<LiquidacionNomina>();
    }

    public class PeriodoNominaFormVm
    {
        [Required]
        public string Descripcion { get; set; } = null!;

        [Required]
        public string TipoPeriodo { get; set; } = "Mensual";

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime? FechaPago { get; set; }

        [Required]
        public string Estado { get; set; } = "Abierto";
    }
}