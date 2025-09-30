using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class Proveedore
{
    public int IdProveedor { get; set; }

    public string? Nit { get; set; }

    public string? RazonSocial { get; set; }

    public string? Direccion { get; set; }

    public string? Celular { get; set; }

    public string? Correo { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
