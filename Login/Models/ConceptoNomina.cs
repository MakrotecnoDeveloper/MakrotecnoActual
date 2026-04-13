using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.Models
{
    public class ConceptoNomina
    {
        public int IdConcepto { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Naturaleza { get; set; } = null!;
        public string ModoCalculo { get; set; } = null!;
        public string? BaseCalculo { get; set; }
        public int? IdServicio { get; set; }
        public decimal? Porcentaje { get; set; }
        public decimal? ValorFijo { get; set; }
        public bool AplicaPrestaciones { get; set; }
        public bool AplicaSeguridadSocial { get; set; }
        public bool AplicaParafiscales { get; set; }
        public int OrdenVisualizacion { get; set; }
        public bool EsActivo { get; set; }
        public DateTime FechaCreacion { get; set; }

        public virtual Servicio? Servicio { get; set; }

        public virtual ICollection<DetalleConceptoEmpleado> DetalleConceptosEmpleado { get; set; } = new List<DetalleConceptoEmpleado>();
        public virtual ICollection<NovedadNomina> NovedadesNomina { get; set; } = new List<NovedadNomina>();
        public virtual ICollection<DetalleLiquidacion> DetallesLiquidacion { get; set; } = new List<DetalleLiquidacion>();
    }

    public class ConceptoNominaFormVm
    {
        [Required]
        [StringLength(30)]
        public string Codigo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Naturaleza { get; set; } = "Ingreso";

        [Required]
        public string ModoCalculo { get; set; } = "Fijo";

        public string? BaseCalculo { get; set; }

        public int? IdServicio { get; set; }

        public decimal? Porcentaje { get; set; }
        public decimal? ValorFijo { get; set; }

        public bool AplicaPrestaciones { get; set; }
        public bool AplicaSeguridadSocial { get; set; }
        public bool AplicaParafiscales { get; set; }

        public int OrdenVisualizacion { get; set; } = 0;
        public bool EsActivo { get; set; } = true;

        public List<SelectListItem> Servicios { get; set; } = new();
    }
}