using System;
using System.Collections.Generic;

namespace MK_ERP.Models;;

public partial class Empleadoempresa
{
    public int IdEmpleadoE { get; set; }

    public int Cedula { get; set; }

    public string IdEmpresa { get; set; } = null!;

    public virtual Empleado CedulaNavigation { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;
}
