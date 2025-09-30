using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class Plataforma
{
    public int IdPlataforma { get; set; }

    public string? NombrePltf { get; set; }

    public virtual ICollection<Plataformasuscripcion> Plataformasuscripcions { get; set; } = new List<Plataformasuscripcion>();
}
