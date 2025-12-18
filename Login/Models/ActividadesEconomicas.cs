namespace Plataforma.Models;
public class ActividadesEconomicas
{
    public int IdActividad { get; set; }
    public string NombreActividad { get; set; }
    public string Descripcion { get; set; }

    public ICollection<Empresas> Empresas { get; set; }
    public ICollection<ModulosActividadEconomica> ModulosActividad { get; set; }
}
