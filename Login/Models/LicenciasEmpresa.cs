namespace Plataforma.Models;
public class LicenciasEmpresa
{
    public int IdLicencia { get; set; }
    public string IdEmpresa { get; set; }
    public int IdPlan { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Estado { get; set; }

    public Empresas Empresa { get; set; }
    public Planes Plan { get; set; }
}
