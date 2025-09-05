using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Ganancia
{
    public int IdGanancia { get; set; }

    public DateTime Fecha { get; set; }

    public decimal Ingresos { get; set; }

    public decimal Costos { get; set; }

    public decimal Utilidad { get; set; }

    public int Cedula { get; set; }

    public virtual Empleado CedulaNavigation { get; set; } = null!;
}
