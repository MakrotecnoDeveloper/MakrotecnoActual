using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class AdicionFactura
{
    public int IdAdicion { get; set; }

    public int IdFactura { get; set; }

    public decimal Valor { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? Fecha { get; set; }

    public int Cedula { get; set; }

    public virtual Factura IdFacturaNavigation { get; set; } = null!;
}
