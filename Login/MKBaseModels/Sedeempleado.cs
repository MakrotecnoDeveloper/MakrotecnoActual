using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class Sedeempleado
{
    public int IdSedeEmpleado { get; set; }

    public int IdSede { get; set; }

    public int Cedula { get; set; }

    public int IdCargo { get; set; }

    public virtual Empleado CedulaNavigation { get; set; } = null!;

    public virtual Tipocargo IdCargoNavigation { get; set; } = null!;

    public virtual Sede IdSedeNavigation { get; set; } = null!;
}
