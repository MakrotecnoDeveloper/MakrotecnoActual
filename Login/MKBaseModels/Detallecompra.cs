using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class Detallecompra
{
    public int IdDetalleCompra { get; set; }

    public int IdCompra { get; set; }

    public string Codigo { get; set; } = null!;

    public decimal Stock { get; set; }

    public decimal Vneto { get; set; }

    public decimal Vtotal { get; set; }

    public decimal Vventa { get; set; }

    public virtual Producto CodigoNavigation { get; set; } = null!;

    public virtual Compra IdCompraNavigation { get; set; } = null!;
}
