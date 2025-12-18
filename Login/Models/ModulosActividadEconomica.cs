namespace Plataforma.Models;
public class ModulosActividadEconomica
{
    public int IdActividad { get; set; }
    public int IdModulo { get; set; }

    public ActividadesEconomicas ActividadEconomica { get; set; }
    public Modulos Modulo { get; set; }
}
