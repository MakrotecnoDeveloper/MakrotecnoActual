using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Diagnosticoproblema
{
    public int? IdOrden { get; set; }

    public string? ProblemaDetectado { get; set; }

    public string? Solucion { get; set; }

    public decimal? Costo { get; set; }

    public decimal? CostoInterno { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int? Cedula { get; set; }

    public int IdDiagProb { get; set; }

    public virtual Empleado? CedulaNavigation { get; set; }

    public virtual Ordenservicio? IdOrdenNavigation { get; set; }
}
