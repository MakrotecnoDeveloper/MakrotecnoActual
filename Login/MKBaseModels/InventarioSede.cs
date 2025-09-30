using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class InventarioSede
{
    public int IdInventario { get; set; }

    public string ProductoId { get; set; } = null!;

    public int SedeId { get; set; }

    public decimal Cantidad { get; set; }

    public int? PrecioUnitario { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public int? Cedula { get; set; }

    public virtual Empleado? CedulaNavigation { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual Sede Sede { get; set; } = null!;
}
