using System;
using System.Collections.Generic;

namespace Plataforma.MKBaseModels;

public partial class OrdenServicio
{
    public int IdDispositivo { get; set; }

    public string? ProblemaReportado { get; set; }

    public string? Estado { get; set; }

    public DateTime FechaIngreso { get; set; }

    public int IdOrden { get; set; }

    public string? Observaciones { get; set; }

    public int Cedula { get; set; }

    public virtual Empleado CedulaNavigation { get; set; } = null!;

    public virtual ICollection<Gestionrealizadum> Gestionrealizada { get; set; } = new List<Gestionrealizadum>();

    public virtual ICollection<HistOrdSer> HistOrdSers { get; set; } = new List<HistOrdSer>();

    public virtual Dispositivo IdDispositivoNavigation { get; set; } = null!;
}
