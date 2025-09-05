using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Detallecompra
{
    public int IdDetalleCompra { get; set; }

    public int IdCompra { get; set; }

    public string CodProducto { get; set; } = null!;

    public decimal Cantidad { get; set; }

    public decimal ValorNeto { get; set; }

    public decimal ValorTotal { get; set; }

    public decimal ValorVenta { get; set; }

    public virtual Producto CodProductoNavigation { get; set; } = null!;

    public virtual Compra IdCompraNavigation { get; set; } = null!;
}
