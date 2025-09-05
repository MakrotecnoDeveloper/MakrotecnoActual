using System;
using System.Collections.Generic;

namespace MK_ERP.Models;

public partial class Ordenservicio
{
    public int? IdDispositivo { get; set; }

    public string? ProblemaReportado { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public int IdOrden { get; set; }

    public virtual ICollection<Diagnosticoproblema> Diagnosticoproblemas { get; set; } = new List<Diagnosticoproblema>();

    public virtual ICollection<Gestionrealizadum> Gestionrealizada { get; set; } = new List<Gestionrealizadum>();

    public virtual Dispositivo? IdDispositivoNavigation { get; set; }

    public virtual ICollection<Seguimientoservicio> Seguimientoservicios { get; set; } = new List<Seguimientoservicio>();
}
