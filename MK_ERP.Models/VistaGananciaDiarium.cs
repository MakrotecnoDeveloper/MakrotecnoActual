using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class VistaGananciaDiarium
{
    public int Cedula { get; set; }

    public decimal? TotalIngresos { get; set; }

    public decimal? TotalCostos { get; set; }

    public decimal? TotalUtilidad { get; set; }
}
