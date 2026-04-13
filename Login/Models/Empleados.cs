using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("Empleados", Schema = "dbo")]
public class Empleados
{
    [Key]
    public int Cedula { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Genero { get; set; }
    public string? Correo { get; set; }
    public string? Rh { get; set; }
    public string? Celular { get; set; }
    public string? Contrasena { get; set; }
    public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
    public virtual ICollection<DetalleConceptoEmpleado> DetalleConceptosEmpleado { get; set; } = new List<DetalleConceptoEmpleado>();
    public virtual ICollection<NovedadNomina> NovedadesNomina { get; set; } = new List<NovedadNomina>();
    public virtual ICollection<LiquidacionNomina> LiquidacionesNomina { get; set; } = new List<LiquidacionNomina>();
}

public class EmpleadoPdvViewModel
{
    public List<Empleados> Empleados { get; set; }
    public List<Infopdv> PuntosDeVenta { get; set; }
}

public class AsignarConceptoEmpleadoVm
{
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un empleado.")]
    public int Cedula { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un concepto.")]
    public int IdConcepto { get; set; }

    public decimal? PorcentajePersonalizado { get; set; }
    public decimal? ValorFijoPersonalizado { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime? FechaFin { get; set; }

    public bool Estado { get; set; } = true;
    public string? Observacion { get; set; }

    public List<SelectListItem> Empleados { get; set; } = new();
    public List<SelectListItem> Conceptos { get; set; } = new();
}
