using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Syncpdv
{
    public int Idsync { get; set; }

    public int InfopdvId { get; set; }

    public int? Estado { get; set; }

    public DateTime? FechaEstado { get; set; }

    public int Cedula { get; set; }

    public virtual Empleado CedulaNavigation { get; set; } = null!;

    public virtual Infopdv Infopdv { get; set; } = null!;
}
