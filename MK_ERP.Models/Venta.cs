using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Venta
{
    public int IdVenta { get; set; }

    public int CedulaCliente { get; set; }

    public string MetodoPago { get; set; } = null!;

    public decimal Total { get; set; }

    public string EstadoVenta { get; set; } = null!;

    public int Cedula { get; set; }

    public DateTime? FechaVenta { get; set; }

    public virtual Cliente CedulaClienteNavigation { get; set; } = null!;

    public virtual Empleado CedulaNavigation { get; set; } = null!;

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
