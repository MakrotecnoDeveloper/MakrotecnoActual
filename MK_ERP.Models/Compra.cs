using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Compra
{
    public int IdCompra { get; set; }

    public int IdProveedor { get; set; }

    public decimal? ValorTotal { get; set; }

    public DateTime? FechaCompra { get; set; }

    public int? Estado { get; set; }

    public string? CodFacturaExterno { get; set; }

    public virtual ICollection<Detallecompra> Detallecompras { get; set; } = new List<Detallecompra>();

    public virtual Proveedore IdProveedorNavigation { get; set; } = null!;
}
