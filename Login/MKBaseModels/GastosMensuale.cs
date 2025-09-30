using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class GastosMensuale
{
    public int IdGasto { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Monto { get; set; }

    public DateOnly Fecha { get; set; }

    public string? Categoria { get; set; }

    public byte Frecuencia { get; set; }

    public string? Notas { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public int? PeriodoYear { get; set; }

    public int? PeriodoMonth { get; set; }
}
