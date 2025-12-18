namespace Plataforma.Models;
public class Planes
{
    public int IdPlan { get; set; }
    public string NombrePlan { get; set; }
    public int LimiteEmpresas { get; set; }
    public int LimiteSedes { get; set; }
    public int LimitePDV { get; set; }
    public int LimiteUsuarios { get; set; }
    public int LimiteProductos { get; set; }
    public int LimiteClientes { get; set; }
    public int LimiteReportes { get; set; }
    public bool IncluyeModulosGenericos { get; set; }
    public decimal PrecioMensual { get; set; }

    public ICollection<LicenciasEmpresa> Licencias { get; set; }
    public ICollection<PlanesModulos> PlanesModulos { get; set; }
}
