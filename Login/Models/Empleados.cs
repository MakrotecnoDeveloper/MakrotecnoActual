using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
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
    // Navegación
    public ICollection<Contratos> Contratos { get; set; } = new List<Contratos>();
    public ICollection<DetalleConceptosEmpleado> Conceptos { get; set; } = new List<DetalleConceptosEmpleado>();
    public ICollection<LiquidacionNomina> Liquidaciones { get; set; } = new List<LiquidacionNomina>();
}

public class EmpleadoPdvViewModel
{
    public List<Empleados> Empleados { get; set; }
    public List<Infopdv> PuntosDeVenta { get; set; }
}
