using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class Logslogin
{
    public int IdLog { get; set; }

    public int Cedula { get; set; }

    public string Correo { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public int Estado { get; set; }

    public int InfopdvId { get; set; }

    public virtual Empleado CedulaNavigation { get; set; } = null!;

    public virtual Infopdv Infopdv { get; set; } = null!;
}
