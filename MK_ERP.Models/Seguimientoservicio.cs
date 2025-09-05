using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Seguimientoservicio
{
    public int? IdOrden { get; set; }

    public DateTime? FechaSeguimiento { get; set; }

    public string? RespuestaCliente { get; set; }

    public string? Estado { get; set; }

    public int IdSeguiServ { get; set; }

    public virtual Ordenservicio? IdOrdenNavigation { get; set; }
}
