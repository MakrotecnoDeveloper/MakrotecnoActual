using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models
{
    public class Contrato
    {
        public int IdContrato { get; set; }
        public int Cedula { get; set; } = 0!;
        public string TipoContrato { get; set; } = null!;
        public string? Cargo { get; set; }
        public string? Area { get; set; }
        public string TipoSalario { get; set; } = "Fijo";
        public decimal SalarioBase { get; set; }
        public bool AuxilioTransporteAplica { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; } = "Activo";
        public string? Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? IdTipoCargo { get; set; }

        public virtual Empleados Empleado { get; set; } = null!;

        public virtual ICollection<LiquidacionNomina> LiquidacionesNomina { get; set; } = new List<LiquidacionNomina>();
        [ForeignKey(nameof(IdTipoCargo))]
        public virtual TipoCargo? TipoCargo { get; set; }
    }

    public class ContratoFormVm
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un empleado.")]
        public int Cedula { get; set; }

        [Required]
        public string TipoContrato { get; set; } = "Indefinido";

        [Required]
        public string TipoSalario { get; set; } = "Variable";

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cargo.")]
        public int IdTipoCargo { get; set; }

        public string? AreaNombre { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El salario no puede ser negativo.")]
        public decimal SalarioBase { get; set; }

        public bool AuxilioTransporteAplica { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime? FechaFin { get; set; }

        [Required]
        public string Estado { get; set; } = "Activo";

        public string? Observaciones { get; set; }

        public List<SelectListItem> Empleados { get; set; } = new();
        public List<SelectListItem> Cargos { get; set; } = new();
    }
}