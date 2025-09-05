using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Factura
{
    public int IdFactura { get; set; }

    public int NumeroFactura { get; set; }

    public int IdVenta { get; set; }

    public DateOnly FechaEmision { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }

    public string EstadoFactura { get; set; } = null!;

    public virtual Venta IdVentaNavigation { get; set; } = null!;
}
