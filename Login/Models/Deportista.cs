using System;
using System.Collections.Generic;

namespace Plataforma.Models;

public class Deportista
{
    public int IdDeportista { get; set; }
    public int IdCliente { get; set; }
    public int IdDeporte { get; set; }
    public string? IdEmpresa { get; set; }
    public int? IdSede { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;

    public Clientes Cliente { get; set; } = null!;
    public Deporte Deporte { get; set; } = null!;
    public ICollection<ResultadoDeportivo> ResultadosDeportivos { get; set; } = new List<ResultadoDeportivo>();
}

