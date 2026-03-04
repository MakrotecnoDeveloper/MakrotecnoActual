using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models;
[Table("OrdenServicios", Schema = "dbo")]
public class OrdenServicios
{
    [Key]
    public int IdOrden { get; set; }
    public int IdDispositivo { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string? ProblemaReportado { get; set; }
    public string? Estado { get; set; } = "Ingresada";
    public Dispositivos? Dispositivo { get; set; }
    public string? Observaciones { get; set; }
    public int? Cedula {  get; set; }
    public decimal? ValorPago { get; set; }
}

public class OrdenServicioRowDTO
{
    public int IdOrden { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string Cliente { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Password { get; set; } = "";
    public string Marca { get; set; } = "";
    public string Modelo { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string Observacion { get; set; } = "";
    public string Estado { get; set; } = "";
    public int? Cedula { get; set; }
}
