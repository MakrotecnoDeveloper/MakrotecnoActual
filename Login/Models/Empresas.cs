using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma.Models;
[Table("empresas", Schema = "dbo")]
public class Empresas
{
    [Key]
    public string? Id_empresa { get; set; }
    public string? Nombre { get; set; }
    public string? Pais { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public int? ActividadEconomicaId { get; set; }
    public string Estado { get; set; }

    public ActividadesEconomicas ActividadEconomica { get; set; }
    public ICollection<LicenciasEmpresa> Licencias { get; set; }
}