using System;
using System.Collections.Generic;

namespace Plataforma.Models;

public class SesionTomaTiempo
{
    public int IdSesionTomaTiempo { get; set; }
    public int IdPruebaDeportiva { get; set; }
    public DateTime FechaSesion { get; set; } = DateTime.Now;
    public string? Observaciones { get; set; }
    public string? IdEmpresa { get; set; }
    public int? IdSede { get; set; }
    public string? UsuarioRegistro { get; set; }

    public PruebaDeportiva PruebaDeportiva { get; set; } = null!;
    public ICollection<ResultadoDeportivo> Resultados { get; set; } = new List<ResultadoDeportivo>();
}

