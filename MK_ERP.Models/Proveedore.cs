using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Proveedore
{
    public int IdProveedor { get; set; }

    public string Nit { get; set; } = null!;

    public string RazonSocial { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Celular { get; set; }

    public string? Correo { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
