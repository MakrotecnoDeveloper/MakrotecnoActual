namespace Plataforma.Models;
public class Modulos
{
    public int IdModulo { get; set; }
    public string NombreModulo { get; set; }
    public string Descripcion { get; set; }
    public bool EsGenerico { get; set; }
    public bool EsEspecializado { get; set; }

    public ICollection<PlanesModulos> PlanesModulos { get; set; }
    public ICollection<ModulosActividadEconomica> ModulosActividad { get; set; }
    public ICollection<MenuOpciones> MenuOpciones { get; set; }
}

