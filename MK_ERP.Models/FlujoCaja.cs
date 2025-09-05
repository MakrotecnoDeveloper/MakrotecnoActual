using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class FlujoCaja
{
    public int IdFlujoCaja { get; set; }

    public DateTime? Fecha { get; set; }

    public string? TipoMovimiento { get; set; }

    public string? Concepto { get; set; }

    public decimal Monto { get; set; }

    public int Cedula { get; set; }

    public virtual Empleado CedulaNavigation { get; set; } = null!;
}
