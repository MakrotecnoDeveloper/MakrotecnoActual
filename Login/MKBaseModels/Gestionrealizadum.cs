using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class Gestionrealizadum
{
    public int? IdOrden { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public string? Estado { get; set; }

    public decimal? CostoTotal { get; set; }

    public int IdGr { get; set; }

    public virtual OrdenServicio? IdOrdenNavigation { get; set; }
}
