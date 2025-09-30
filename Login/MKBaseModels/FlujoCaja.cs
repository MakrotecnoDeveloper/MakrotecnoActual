using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class FlujoCaja
{
    public int IdFlujoCaja { get; set; }

    public DateTime? Fecha { get; set; }

    public string? TipoMovimiento { get; set; }

    public string? Concepto { get; set; }

    public decimal Monto { get; set; }

    public int Cedula { get; set; }

    public string? ConceptosJson { get; set; }

    public decimal? Efectivo { get; set; }

    public decimal? Transferencia { get; set; }

    public decimal? GastosEfectivo { get; set; }

    public decimal? GastosTransferencia { get; set; }

    public decimal? Diferencia { get; set; }

    public virtual Empleado CedulaNavigation { get; set; } = null!;
}
