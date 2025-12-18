namespace Plataforma.Models;
public class PlanesModulos
{
    public int IdPlan { get; set; }
    public int IdModulo { get; set; }

    public Planes Plan { get; set; }
    public Modulos Modulo { get; set; }
}

