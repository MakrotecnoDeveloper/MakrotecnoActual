using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class CierreCaja
{
    public int IdCierreCaja { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal TotalVentas { get; set; }

    public decimal EfectivoReal { get; set; }

    public decimal Diferencia { get; set; }

    public int Cedula { get; set; }

    public virtual Empleado CedulaNavigation { get; set; } = null!;
}
