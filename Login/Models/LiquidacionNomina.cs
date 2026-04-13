using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models
{
    public class LiquidacionNomina
    {
        public int IdLiquidacion { get; set; }
        public int Cedula { get; set; } = 0!;
        public int IdContrato { get; set; }
        public int IdPeriodo { get; set; }
        public DateTime FechaLiquidacion { get; set; }
        public DateTime? FechaPago { get; set; }
        public decimal TotalDevengado { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal NetoPagar { get; set; }
        public string Estado { get; set; } = "Borrador";
        public string? UsuarioLiquida { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }

        public virtual Empleados Empleado { get; set; } = null!;
        public virtual Contrato Contrato { get; set; } = null!;
        public virtual PeriodoNomina PeriodoNomina { get; set; } = null!;

        public virtual ICollection<DetalleLiquidacion> DetalleLiquidacion { get; set; } = new List<DetalleLiquidacion>();
    }

    public class GenerarLiquidacionVm
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un empleado.")]
        public int Cedula { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un período.")]
        public int IdPeriodo { get; set; }

        public List<SelectListItem> Empleados { get; set; } = new();
        public List<SelectListItem> Periodos { get; set; } = new();
    }
}