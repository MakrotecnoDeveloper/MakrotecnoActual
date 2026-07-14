using System;

namespace Plataforma.Models;

public class ResultadoDeportivo
{
    public int IdResultadoDeportivo { get; set; }
    public int IdSesionTomaTiempo { get; set; }
    public int IdDeportista { get; set; }
    public TimeSpan Tiempo { get; set; }
    public int? Posicion { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public string? Observaciones { get; set; }

    public SesionTomaTiempo SesionTomaTiempo { get; set; } = null!;
    public Deportista Deportista { get; set; } = null!;

}

