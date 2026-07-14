using System.Collections.Generic;

namespace Plataforma.Models;

public class PruebaDeportiva
{
    public int IdPruebaDeportiva { get; set; }
    public int IdDeporte { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal? Distancia { get; set; }
    public string? UnidadMedida { get; set; }
    public string? Modalidad { get; set; }
    public bool Activo { get; set; } = true;

    public Deporte Deporte { get; set; } = null!;
    public ICollection<SesionTomaTiempo> SesionesTomaTiempo { get; set; } = new List<SesionTomaTiempo>();

}

