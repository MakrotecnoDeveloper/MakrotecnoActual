using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class PlataformaStrm
{
    public int IdPlataforma { get; set; }

    public string? NombrePltf { get; set; }

    public virtual ICollection<Plataformasuscripcion> Plataformasuscripcions { get; set; } = new List<Plataformasuscripcion>();
}
