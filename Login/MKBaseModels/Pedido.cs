using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class Pedido
{
    public int IdPedido { get; set; }

    public int IdVenta { get; set; }

    public string Codigo { get; set; } = null!;

    public decimal Stock { get; set; }

    public int Vneto { get; set; }

    public int Vventa { get; set; }

    public int InfopdvId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Producto CodigoNavigation { get; set; } = null!;

    public virtual Venta IdVentaNavigation { get; set; } = null!;

    public virtual Infopdv Infopdv { get; set; } = null!;
}
