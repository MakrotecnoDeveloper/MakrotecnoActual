using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class HistOrdSer
{
    public int IdOrden { get; set; }

    public string? ReparacionDet { get; set; }

    public DateTime FechaRegistro { get; set; }

    public int IdDiagProb { get; set; }

    public string? CodProducto { get; set; }

    public int Cedula { get; set; }

    public virtual Producto? CodProductoNavigation { get; set; }

    public virtual OrdenServicio IdOrdenNavigation { get; set; } = null!;
}
